
// ===================================================================
// URL 查询参数解析工具
// ===================================================================
function getQueryParam(name, defaultValue) {
    const params = new URLSearchParams(window.location.search);
    return params.has(name) ? params.get(name) : defaultValue;
}

function getAllQueryParams() {
    const params = new URLSearchParams(window.location.search);
    const result = {};
    for (const [key, value] of params.entries()) {
        result[key] = value;
    }
    return result;
}


// ===================================================================
// 通用 fetch POST 工具函数
// ===================================================================
async function postData(url, data, options = {}) {
    const { timeout = 10000, headers = {} } = options;

    const controller = new AbortController();
    const timeoutId = setTimeout(() => controller.abort(), timeout);

    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                ...headers
            },
            body: JSON.stringify(data),
            signal: controller.signal
        });

        clearTimeout(timeoutId);

        if (!response.ok) {
            throw new Error(`HTTP ${response.status}: ${response.statusText}`);
        }

        const contentType = response.headers.get('content-type') || '';
        if (contentType.includes('application/json')) {
            return await response.json();
        }
        return await response.text();
    } catch (err) {
        clearTimeout(timeoutId);
        if (err.name === 'AbortError') {
            throw new Error(`请求超时（${timeout}ms）: ${url}`);
        }
        throw err;
    }
}




(async function () {
    // ===== 单一 WebGL Canvas，所有内容（线条/圆形/圆弧/矩形/多边形/文字）均由 GPU 渲染 =====


    const filepath = getQueryParam('f', '');
    const area = getQueryParam('a', '');

    await postData(`/app/open?f=${encodeURIComponent(filepath)}&a=${encodeURIComponent(area)}`, { f: filepath, a: area })
        .then(result => {

            console.log('保存成功:', result);
            drawPid(result);
        }
        )
        .catch(err =>
            console.error('保存失败:', err)
        );



})();


















function drawPid(dataJson) {

    if (!dataJson.Success) {
        return;
    }


    let data = dataJson.Data;

    if (!data.PolyLines || !Array.isArray(data.PolyLines)) {

        console.error('数据格式错误，缺少 Polylines 数组');
    }








    const glcanvas = document.getElementById('glcanvas');
    const gl = glcanvas.getContext('webgl') || glcanvas.getContext('experimental-webgl');

    if (!gl) {
        alert('当前浏览器不支持 WebGL，请更换浏览器。');
        return;
    }

    gl.enable(gl.BLEND);
    gl.blendFunc(gl.SRC_ALPHA, gl.ONE_MINUS_SRC_ALPHA);

    // 视图参数
    let scale = 0.3;
    let offsetX = 0;
    let offsetY = 0;
    let isDragging = false;
    let lastX, lastY;

    // 渲染参数
    const LINE_COLOR = [0.0, 1.0, 0.0, 1.0];
    const WORLD_SIZE = data.Height;

    // ===================================================================
    // 着色器程序 1：批量线条（世界坐标真实缩放，用于海量折线）
    // ===================================================================
    const lineVS = `
    attribute vec2 aPosition;
    uniform vec2 uOffset;
    uniform float uScale;
    uniform vec2 uResolution;
    void main() {
      vec2 screenPos = (aPosition + uOffset) * uScale;
      vec2 clip = vec2(
        screenPos.x / uResolution.x * 2.0 - 1.0,
        1.0 - screenPos.y / uResolution.y * 2.0
      );
      gl_Position = vec4(clip, 0.0, 1.0);
    }
  `;
    const solidFS = `
    precision mediump float;
    uniform vec4 uColor;
    void main() { gl_FragColor = uColor; }
  `;

    // ===================================================================
    // 着色器程序 2：广告牌图形（几何顶点是像素偏移，屏幕尺寸恒定，不随缩放变形）
    // 用于：可交互元素圆形、示例圆弧/矩形/多边形
    // ===================================================================
    const shapeVS = `
    attribute vec2 aLocalOffset;
    uniform vec2 uCenter;
    uniform vec2 uOffset;
    uniform float uScale;
    uniform vec2 uResolution;
    void main() {
      vec2 screenPos = (uCenter + uOffset) * uScale + aLocalOffset * uScale;
      vec2 clip = vec2(
        screenPos.x / uResolution.x * 2.0 - 1.0,
        1.0 - screenPos.y / uResolution.y * 2.0
      );
      gl_Position = vec4(clip, 0.0, 1.0);
    }
  `;

    // ===================================================================
    // 着色器程序 3：文字贴图（同样是广告牌四边形，采样纹理）
    // ===================================================================
    const textVS = `
    attribute vec2 aLocalOffset;
    attribute vec2 aTexCoord;
    uniform vec2 uCenter;
    uniform vec2 uOffset;
    uniform float uScale;
    uniform vec2 uResolution;
    varying vec2 vTexCoord;
    void main() {
      vec2 screenPos = (uCenter + uOffset) * uScale + aLocalOffset * uScale;
      vec2 clip = vec2(
        screenPos.x / uResolution.x * 2.0 - 1.0,
        1.0 - screenPos.y / uResolution.y * 2.0
      );
      gl_Position = vec4(clip, 0.0, 1.0);
      vTexCoord = aTexCoord;
    }
  `;
    const textFS = `
    precision mediump float;
    uniform sampler2D uSampler;
    varying vec2 vTexCoord;
    void main() { gl_FragColor = texture2D(uSampler, vTexCoord); }
  `;

    function compileShader(type, src) {
        const shader = gl.createShader(type);
        gl.shaderSource(shader, src);
        gl.compileShader(shader);
        if (!gl.getShaderParameter(shader, gl.COMPILE_STATUS)) {
            console.error('着色器编译失败:', gl.getShaderInfoLog(shader));
            gl.deleteShader(shader);
            return null;
        }
        return shader;
    }

    function createProgram(vsSrc, fsSrc) {
        const vs = compileShader(gl.VERTEX_SHADER, vsSrc);
        const fs = compileShader(gl.FRAGMENT_SHADER, fsSrc);
        const program = gl.createProgram();
        gl.attachShader(program, vs);
        gl.attachShader(program, fs);
        gl.linkProgram(program);
        if (!gl.getProgramParameter(program, gl.LINK_STATUS)) {
            console.error('着色器程序链接失败:', gl.getProgramInfoLog(program));
        }
        return program;
    }

    const lineProgram = createProgram(lineVS, solidFS);
    const lineLoc = {
        aPosition: gl.getAttribLocation(lineProgram, 'aPosition'),
        uOffset: gl.getUniformLocation(lineProgram, 'uOffset'),
        uScale: gl.getUniformLocation(lineProgram, 'uScale'),
        uResolution: gl.getUniformLocation(lineProgram, 'uResolution'),
        uColor: gl.getUniformLocation(lineProgram, 'uColor')
    };

    const shapeProgram = createProgram(shapeVS, solidFS);
    const shapeLoc = {
        aLocalOffset: gl.getAttribLocation(shapeProgram, 'aLocalOffset'),
        uCenter: gl.getUniformLocation(shapeProgram, 'uCenter'),
        uOffset: gl.getUniformLocation(shapeProgram, 'uOffset'),
        uScale: gl.getUniformLocation(shapeProgram, 'uScale'),
        uResolution: gl.getUniformLocation(shapeProgram, 'uResolution'),
        uColor: gl.getUniformLocation(shapeProgram, 'uColor')
    };

    const textProgram = createProgram(textVS, textFS);
    const textLoc = {
        aLocalOffset: gl.getAttribLocation(textProgram, 'aLocalOffset'),
        aTexCoord: gl.getAttribLocation(textProgram, 'aTexCoord'),
        uCenter: gl.getUniformLocation(textProgram, 'uCenter'),
        uOffset: gl.getUniformLocation(textProgram, 'uOffset'),
        uScale: gl.getUniformLocation(textProgram, 'uScale'),
        uResolution: gl.getUniformLocation(textProgram, 'uResolution'),
        uSampler: gl.getUniformLocation(textProgram, 'uSampler')
    };

    // ===== 数据：10000 条随机多段线（沿用之前的性能验证数据） =====
    let polylines = generateRandomPolylines(10000, WORLD_SIZE);
    polylines = generateDataPolylines();



    function generateRandomPolylines(count, worldSize) {
        const result = [];
        for (let i = 0; i < count; i++) {
            const pointCount = 3 + Math.floor(Math.random() * 4);
            const points = [];
            let x = Math.random() * worldSize;
            let y = Math.random() * worldSize;
            for (let j = 0; j < pointCount; j++) {
                points.push({ x, y });
                x += (Math.random() - 0.5) * 400;
                y += (Math.random() - 0.5) * 400;
                x = Math.max(0, Math.min(worldSize, x));
                y = Math.max(0, Math.min(worldSize, y));
            }
            result.push(points);
        }
        return result;
    }

    function generateDataPolylines() {
        const result = [];
        for (let i = 0; i < data.PolyLines.length; i++) {
            var p = data.PolyLines[i];

            var color = p.Color;
            var ltpoints = p.Points;

            ltpoints.forEach(points => {
                result.push(points);
            });


        }
        return result;
    }





    let segmentVertexCount = 0;
    const lineVertexBuffer = gl.createBuffer();
    function buildLineBuffer(lines) {
        let total = 0;
        for (let i = 0; i < lines.length; i++) total += Math.max(0, lines[i].length - 1);
        const data = new Float32Array(total * 4);
        let ptr = 0;
        for (let i = 0; i < lines.length; i++) {
            const pts = lines[i];
            for (let j = 1; j < pts.length; j++) {
                data[ptr++] = pts[j - 1].x;
                data[ptr++] = pts[j - 1].y;
                data[ptr++] = pts[j].x;
                data[ptr++] = pts[j].y;
            }
        }
        gl.bindBuffer(gl.ARRAY_BUFFER, lineVertexBuffer);
        gl.bufferData(gl.ARRAY_BUFFER, data, gl.STATIC_DRAW);
        segmentVertexCount = total * 2;
    }
    buildLineBuffer(polylines);

    // ===================================================================
    // 几何生成工具：均以“像素偏移”为单位，供广告牌 shader 使用
    // ===================================================================
    function createGLBuffer(floatArray) {
        const buf = gl.createBuffer();
        gl.bindBuffer(gl.ARRAY_BUFFER, buf);
        gl.bufferData(gl.ARRAY_BUFFER, floatArray, gl.STATIC_DRAW);
        return buf;
    }

    // 圆形：返回填充（TRIANGLE_FAN，含圆心）与描边（LINE_LOOP，仅周长点）两组几何
    function makeCircleGeometry(radiusPx, segments) {
        segments = segments || 40;
        const fill = [0, 0];
        const stroke = [];
        for (let i = 0; i <= segments; i++) {
            const a = (i / segments) * Math.PI * 2;
            const x = Math.cos(a) * radiusPx;
            const y = Math.sin(a) * radiusPx;
            fill.push(x, y);
            if (i < segments) stroke.push(x, y);
        }
        return {
            fill: new Float32Array(fill),
            fillCount: segments + 2,
            stroke: new Float32Array(stroke),
            strokeCount: segments
        };
    }

    // 圆弧/扇形：startAngle/endAngle 单位为弧度。填充为扇形（TRIANGLE_FAN 含圆心），描边仅弧线（LINE_STRIP）
    function makeArcGeometry(radiusPx, startAngle, endAngle, segments) {
        segments = segments || 32;
        const fill = [0, 0];
        const stroke = [];
        for (let i = 0; i <= segments; i++) {
            const a = startAngle + (endAngle - startAngle) * (i / segments);
            const x = Math.cos(a) * radiusPx;
            const y = Math.sin(a) * radiusPx;
            fill.push(x, y);
            stroke.push(x, y);
        }
        return {
            fill: new Float32Array(fill),
            fillCount: segments + 2,
            stroke: new Float32Array(stroke),
            strokeCount: segments + 1
        };
    }

    // 矩形：中心为锚点，宽高为像素单位
    function makeRectGeometry(width, height) {
        const hw = width / 2;
        const hh = height / 2;
        const pts = [-hw, -hh, hw, -hh, hw, hh, -hw, hh];
        return {
            fill: new Float32Array(pts),
            fillCount: 4,
            stroke: new Float32Array(pts),
            strokeCount: 4
        };
    }

    // 任意凸多边形：传入 [[x,y], ...]（像素偏移，相对锚点）
    function makePolygonGeometry(pointsPx) {
        const arr = [];
        for (let i = 0; i < pointsPx.length; i++) arr.push(pointsPx[i][0], pointsPx[i][1]);
        const data = new Float32Array(arr);
        return { fill: data, fillCount: pointsPx.length, stroke: data, strokeCount: pointsPx.length };
    }

    function drawShapeBuffer(buffer, count, mode, centerX, centerY, colorRGBA) {
        gl.useProgram(shapeProgram);
        gl.bindBuffer(gl.ARRAY_BUFFER, buffer);
        gl.enableVertexAttribArray(shapeLoc.aLocalOffset);
        gl.vertexAttribPointer(shapeLoc.aLocalOffset, 2, gl.FLOAT, false, 0, 0);
        gl.uniform2f(shapeLoc.uCenter, centerX, centerY);
        gl.uniform2f(shapeLoc.uOffset, offsetX, offsetY);
        gl.uniform1f(shapeLoc.uScale, scale);
        gl.uniform2f(shapeLoc.uResolution, viewportCssWidth, viewportCssHeight);
        gl.uniform4fv(shapeLoc.uColor, colorRGBA);
        gl.drawArrays(mode, 0, count);
    }

    // ===================================================================
    // 文字贴图：用离屏（不挂载到 DOM）2D canvas 光栅化文字，再作为纹理贴到 GPU 四边形上
    // ===================================================================
    const offscreen = document.createElement('canvas');
    const offctx = offscreen.getContext('2d');

    function createTextTexture(text, fontPx, cssColor) {
        offctx.font = `${fontPx}px sans-serif`;
        offctx.textBaseline = 'alphabetic'; // 保证 measureText 的 ascent/descent 相对基线一致
        const metrics = offctx.measureText(text);
        const padding = 4;

        // 优先使用文字的实际视觉高度（actualBoundingBoxAscent/Descent），
        // 这样带有较高/较低字符（如中文、带下伸的英文字母 g/y）时高度也是准确的；
        // 部分浏览器可能不支持这两个属性，此时回退到 fontPx*1.4 的经验估算值。
        const hasBoundingBox =
            typeof metrics.actualBoundingBoxAscent === 'number' &&
            typeof metrics.actualBoundingBoxDescent === 'number';
        const textHeight = hasBoundingBox
            ? metrics.actualBoundingBoxAscent + metrics.actualBoundingBoxDescent
            : fontPx * 1.4;
        // 实际视觉高度相对于绘制基线的上下偏移，用于按视觉包围盒精确居中绘制文字
        const ascent = hasBoundingBox ? metrics.actualBoundingBoxAscent : fontPx * 0.7;
        const descent = hasBoundingBox ? metrics.actualBoundingBoxDescent : fontPx * 0.7;

        const w = Math.ceil(metrics.width) + padding * 2;
        const h = Math.ceil(textHeight) + padding * 2;

        offscreen.width = w;
        offscreen.height = h;
        // 重新设置尺寸后 context 状态会重置，需要重新设置字体
        offctx.font = `${fontPx}px sans-serif`;
        offctx.clearRect(0, 0, w, h);
        offctx.fillStyle = cssColor;
        offctx.textAlign = 'center';
        // 用 alphabetic 基线 + 手动计算的 baselineY，精确按实际视觉包围盒居中
        // （'middle' 基线是按字体整体行高粗略居中，遇到中文/带下伸字符时会偏差）
        offctx.textBaseline = 'alphabetic';
        // 让文字的实际视觉包围盒（ascent 到 descent）在画布内上下留白均为 padding，
        // 从而真正按视觉高度居中（而不是用字体行高粗略估算居中）
        const baselineY = padding + ascent;
        offctx.fillText(text, w / 2, baselineY);

        const texture = gl.createTexture();
        gl.bindTexture(gl.TEXTURE_2D, texture);
        // 注意：这里不能设置 UNPACK_FLIP_Y_WEBGL = true。
        // 顶点数据中 -hh（局部偏移 y 负值，对应屏幕上方，因为我们的裁剪空间公式
        // 是 1.0 - screenPos.y/height*2，y 越小越靠上）对应纹理坐标 v=0，
        // 这正好和 canvas 图像第 0 行（图像最顶部）一致；
        // 如果再开启 FLIP_Y，会导致纹理上下颠倒一次，和文字本身叠加后变成倒像。
        gl.pixelStorei(gl.UNPACK_FLIP_Y_WEBGL, false);
        gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, offscreen);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.LINEAR);

        // 四边形几何：中心对齐锚点，包含像素偏移 + 纹理坐标（交错存储）
        const hw = w / 2;
        const hh = h / 2;
        const interleaved = new Float32Array([
            -hw, -hh, 0, 0,
            hw, -hh, 1, 0,
            hw, hh, 1, 1,
            -hw, hh, 0, 1
        ]);
        const buffer = createGLBuffer(interleaved);

        return { texture, buffer, vertexCount: 4, width: w, height: h };
    }

    function drawTextLabel(label, centerX, centerY) {
        gl.useProgram(textProgram);
        gl.bindBuffer(gl.ARRAY_BUFFER, label.buffer);
        const stride = 4 * 4; // 4 个 float 一组，每个 float 4 字节
        gl.enableVertexAttribArray(textLoc.aLocalOffset);
        gl.vertexAttribPointer(textLoc.aLocalOffset, 2, gl.FLOAT, false, stride, 0);
        gl.enableVertexAttribArray(textLoc.aTexCoord);
        gl.vertexAttribPointer(textLoc.aTexCoord, 2, gl.FLOAT, false, stride, 2 * 4);

        gl.activeTexture(gl.TEXTURE0);
        gl.bindTexture(gl.TEXTURE_2D, label.texture);
        gl.uniform1i(textLoc.uSampler, 0);

        gl.uniform2f(textLoc.uCenter, centerX, centerY);
        gl.uniform2f(textLoc.uOffset, offsetX, offsetY);
        gl.uniform1f(textLoc.uScale, scale);
        gl.uniform2f(textLoc.uResolution, viewportCssWidth, viewportCssHeight);

        gl.drawArrays(gl.TRIANGLE_FAN, 0, label.vertexCount);
    }

    // ===================================================================
    // 10 个可交互元素（圆形，可拖拽/点击切换状态），全部由 WebGL 绘制
    // ===================================================================
    const ELEMENT_RADIUS = 45;
    const elementCircle = makeCircleGeometry(ELEMENT_RADIUS, 40);
    const elementFillBuffer = createGLBuffer(elementCircle.fill);
    const elementStrokeBuffer = createGLBuffer(elementCircle.stroke);

    const elements = generateInteractiveElements(10, WORLD_SIZE);
    let hoverElement = null;
    let selectedElement = null;

    function generateInteractiveElements(count, worldSize) {
        const cols = Math.ceil(Math.sqrt(count));
        const spacing = worldSize / (cols + 1);
        const list = [];
        for (let i = 0; i < count; i++) {
            const col = i % cols;
            const row = Math.floor(i / cols);
            list.push({
                id: i,
                x: spacing * (col + 1),
                y: spacing * (row + 1),
                colorActive: [0.0, 0.8, 0.4, 1.0],
                colorNormal: [1.0, 0.667, 0.0, 1.0],
                label: createTextTexture('E' + (i + 1), 60, '#ffffff'),
                active: false
            });
        }
        return list;
    }

    // ===================================================================
    // 示例几何图形（文字 / 圆弧 / 矩形 / 多边形），静态演示，不参与交互
    // 放置在世界坐标右上角一片区域，便于平移过去查看
    // ===================================================================
    const demoOriginX = WORLD_SIZE - 500;
    const demoOriginY = 300;

    const demoTitle = createTextTexture('WebGL 几何图形示例：圆弧 / 矩形 / 多边形 / 文字', 80, '#ffff66');

    // 圆弧（扇形，270°）
    const demoArc = makeArcGeometry(200, -Math.PI * 0.75, Math.PI * 0.75, 32);
    const demoArcFillBuffer = createGLBuffer(demoArc.fill);
    const demoArcStrokeBuffer = createGLBuffer(demoArc.stroke);
    const demoArcColor = [0.2, 0.6, 1.0, 0.6];
    const demoArcStrokeColor = [0.4, 0.8, 1.0, 1.0];
    const demoArcCenter = { x: demoOriginX, y: demoOriginY + 350 };
    const demoArcLabel = createTextTexture('圆弧 Arc', 40, '#ffffff');

    // 矩形
    const demoRect = makeRectGeometry(450, 260);
    const demoRectFillBuffer = createGLBuffer(demoRect.fill);
    const demoRectStrokeBuffer = createGLBuffer(demoRect.stroke);
    const demoRectColor = [1.0, 0.4, 0.4, 0.5];
    const demoRectStrokeColor = [1.0, 0.6, 0.6, 1.0];
    const demoRectCenter = { x: demoOriginX + 700, y: demoOriginY + 350 };
    const demoRectLabel = createTextTexture('矩形 Rect', 40, '#ffffff');

    // 多边形（正六边形）
    const hexPoints = [];
    for (let i = 0; i < 6; i++) {
        const a = (i / 6) * Math.PI * 2;
        hexPoints.push([Math.cos(a) * 220, Math.sin(a) * 220]);
    }
    const demoHex = makePolygonGeometry(hexPoints);
    const demoHexFillBuffer = createGLBuffer(demoHex.fill);
    const demoHexColor = [0.6, 1.0, 0.4, 0.5];
    const demoHexStrokeColor = [0.7, 1.0, 0.5, 1.0];
    const demoHexCenter = { x: demoOriginX + 1400, y: demoOriginY + 350 };
    const demoHexLabel = createTextTexture('多边形 Polygon', 40, '#ffffff');

    // ===================================================================

    const polylineModels = []
    for (let i = 0; i < data.PolyLines.length; i++) {
        var p = data.PolyLines[i];

        var color = p.Color;
        var ltpoints = p.Points;

        ltpoints.forEach(points => {
            var _hex = makePolygonGeometry(points);
            var _hexFillBuffer = createGLBuffer(_hex.fill);
            var _hexColor = color;
            var _hexStrokeColor = color;
            var _hexCenter = { x: points[0][0], y: points[0][1] };
            var model = {
                Hex: _hex,
                HexFillBuffer: _hexFillBuffer,
                HexColor: _hexColor,
                HexStrokeColor: _hexStrokeColor,
                HexCenter: _hexCenter
            }
            polylineModels.push(model);
        });


    }





    // 独立圆形（不可交互，纯展示）
    const demoCircle = makeCircleGeometry(180, 48);
    const demoCircleFillBuffer = createGLBuffer(demoCircle.fill);
    const demoCircleStrokeBuffer = createGLBuffer(demoCircle.stroke);
    const demoCircleColor = [1.0, 1.0, 0.4, 0.5];
    const demoCircleStrokeColor = [1.0, 1.0, 0.6, 1.0];
    const demoCircleCenter = { x: demoOriginX + 2100, y: demoOriginY + 350 };
    const demoCircleLabel = createTextTexture('圆形 Circle', 40, '#ffffff');

    // ===================================================================
    // rAF 合帧调度
    // ===================================================================
    let drawScheduled = false;
    function scheduleDraw() {
        if (drawScheduled) return;
        drawScheduled = true;
        requestAnimationFrame(() => {
            drawScheduled = false;
            draw();
        });
    }

    // ===================================================================
    // 画布尺寸 / DPR
    // ===================================================================
    let viewportCssWidth = window.innerWidth;
    let viewportCssHeight = window.innerHeight;

    function resizeCanvas() {
        const dpr = window.devicePixelRatio || 1;
        viewportCssWidth = window.innerWidth;
        viewportCssHeight = window.innerHeight;

        glcanvas.width = viewportCssWidth * dpr;
        glcanvas.height = viewportCssHeight * dpr;
        glcanvas.style.width = viewportCssWidth + 'px';
        glcanvas.style.height = viewportCssHeight + 'px';
        gl.viewport(0, 0, glcanvas.width, glcanvas.height);

        draw();
    }
    window.addEventListener('resize', resizeCanvas);
    resizeCanvas();

    // ===================================================================
    // 坐标换算 & 命中测试（纯数学计算，不依赖 2D Canvas）
    // ===================================================================
    function screenToWorld(clientX, clientY) {
        const rect = glcanvas.getBoundingClientRect();
        const sx = clientX - rect.left;
        const sy = clientY - rect.top;
        return {
            x: sx / scale - offsetX,
            y: sy / scale - offsetY
        };
    }

    function hitTestElement(worldX, worldY) {
        const hitRadius = ELEMENT_RADIUS; // 图形现在随视图真实缩放，命中半径直接用世界坐标半径
        for (let i = elements.length - 1; i >= 0; i--) {
            const el = elements[i];
            const dx = worldX - el.x;
            const dy = worldY - el.y;
            if (dx * dx + dy * dy <= hitRadius * hitRadius) return el;
        }
        return null;
    }

    // ===================================================================
    // 主绘制函数
    // ===================================================================
    function draw() {
        gl.clearColor(0, 0, 0, 1);
        gl.clear(gl.COLOR_BUFFER_BIT);

        // 1) 海量线条：world-space 真实缩放，一次 draw call


        gl.useProgram(lineProgram);
        gl.bindBuffer(gl.ARRAY_BUFFER, lineVertexBuffer);
        gl.enableVertexAttribArray(lineLoc.aPosition);
        gl.vertexAttribPointer(lineLoc.aPosition, 2, gl.FLOAT, false, 0, 0);
        gl.uniform2f(lineLoc.uOffset, offsetX, offsetY);
        gl.uniform1f(lineLoc.uScale, scale);
        gl.uniform2f(lineLoc.uResolution, viewportCssWidth, viewportCssHeight);
        gl.uniform4fv(lineLoc.uColor, LINE_COLOR);
        gl.drawArrays(gl.LINES, 0, segmentVertexCount);



        // 2) 示例几何图形（圆弧/矩形/多边形/圆形），广告牌方式渲染，尺寸恒定不随缩放变形
        // drawShapeBuffer(demoArcFillBuffer, demoArc.fillCount, gl.TRIANGLE_FAN, demoArcCenter.x, demoArcCenter.y, demoArcColor);
        // drawShapeBuffer(demoArcStrokeBuffer, demoArc.strokeCount, gl.LINE_STRIP, demoArcCenter.x, demoArcCenter.y, demoArcStrokeColor);
        // drawTextLabel(demoArcLabel, demoArcCenter.x, demoArcCenter.y + 280);

        // drawShapeBuffer(demoRectFillBuffer, demoRect.fillCount, gl.TRIANGLE_FAN, demoRectCenter.x, demoRectCenter.y, demoRectColor);
        // drawShapeBuffer(demoRectStrokeBuffer, demoRect.strokeCount, gl.LINE_LOOP, demoRectCenter.x, demoRectCenter.y, demoRectStrokeColor);
        // drawTextLabel(demoRectLabel, demoRectCenter.x, demoRectCenter.y + 220);

        // drawShapeBuffer(demoHexFillBuffer, demoHex.fillCount, gl.TRIANGLE_FAN, demoHexCenter.x, demoHexCenter.y, demoHexColor);
        drawShapeBuffer(demoHexFillBuffer, demoHex.strokeCount, gl.LINE_LOOP, demoHexCenter.x, demoHexCenter.y, demoHexStrokeColor);


        // polylineModels.forEach(pp => {
        //     // drawShapeBuffer(pp.HexFillBuffer, pp.Hex.fillCount, gl.TRIANGLE_FAN, pp.HexCenter.x, pp.HexCenter.y, pp.HexColor);
        //     drawShapeBuffer(pp.HexFillBuffer, pp.Hex.strokeCount, gl.LINE_LOOP, pp.HexCenter.x, pp.HexCenter.y, pp.HexStrokeColor);

        // });

        // drawTextLabel(demoHexLabel, demoHexCenter.x, demoHexCenter.y + 280);

        // drawShapeBuffer(demoCircleFillBuffer, demoCircle.fillCount, gl.TRIANGLE_FAN, demoCircleCenter.x, demoCircleCenter.y, demoCircleColor);
        // drawShapeBuffer(demoCircleStrokeBuffer, demoCircle.strokeCount, gl.LINE_LOOP, demoCircleCenter.x, demoCircleCenter.y, demoCircleStrokeColor);
        // drawTextLabel(demoCircleLabel, demoCircleCenter.x, demoCircleCenter.y + 240);

        // drawTextLabel(demoTitle, demoOriginX + 1000, demoOriginY - 120);

        // 3) 10 个可交互元素（圆形 + 描边 + 文字标签）
        for (const el of elements) {
            const isHover = el === hoverElement;
            const isSelected = el === selectedElement;
            const fillColor = el.active ? el.colorActive : el.colorNormal;
            const strokeColor = isSelected ? [1, 1, 1, 1] : isHover ? [1, 1, 0, 1] : [0.2, 0.2, 0.2, 1];

            drawShapeBuffer(elementFillBuffer, elementCircle.fillCount, gl.TRIANGLE_FAN, el.x, el.y, fillColor);
            drawShapeBuffer(elementStrokeBuffer, elementCircle.strokeCount, gl.LINE_LOOP, el.x, el.y, strokeColor);
            drawTextLabel(el.label, el.x, el.y);
        }
    }

    // ===================================================================
    // 鼠标交互
    // ===================================================================
    glcanvas.addEventListener('mousedown', (e) => {
        const world = screenToWorld(e.clientX, e.clientY);
        const hit = hitTestElement(world.x, world.y);

        if (hit) {
            selectedElement = hit;
            hit.active = !hit.active;
        } else {
            isDragging = true;
            lastX = e.clientX;
            lastY = e.clientY;
        }
        scheduleDraw();
    });

    window.addEventListener('mouseup', () => {
        isDragging = false;
    });

    window.addEventListener('mousemove', (e) => {
        if (isDragging) {
            offsetX += (e.clientX - lastX) / scale;
            offsetY += (e.clientY - lastY) / scale;
            lastX = e.clientX;
            lastY = e.clientY;
            scheduleDraw();
            return;
        }

        const world = screenToWorld(e.clientX, e.clientY);
        const hit = hitTestElement(world.x, world.y);
        if (hit !== hoverElement) {
            hoverElement = hit;
            glcanvas.style.cursor = hit ? 'pointer' : 'default';
            scheduleDraw();
        }
    });

    glcanvas.addEventListener(
        'wheel',
        (e) => {
            e.preventDefault();
            const zoomFactor = 1.05;
            const mouseX = e.offsetX / scale - offsetX;
            const mouseY = e.offsetY / scale - offsetY;

            if (e.deltaY < 0) scale *= zoomFactor;
            else scale /= zoomFactor;

            offsetX -= mouseX - (e.offsetX / scale - offsetX);
            offsetY -= mouseY - (e.offsetY / scale - offsetY);
            scheduleDraw();
        },
        { passive: false }
    );

    // 触摸缩放（双指）/ 平移（单指）
    let lastTouchDist = null;
    glcanvas.addEventListener(
        'touchstart',
        (e) => {
            if (e.touches.length === 2) {
                lastTouchDist = getTouchDist(e.touches);
            } else if (e.touches.length === 1) {
                lastX = e.touches[0].clientX;
                lastY = e.touches[0].clientY;
            }
        },
        { passive: false }
    );

    glcanvas.addEventListener(
        'touchmove',
        (e) => {
            e.preventDefault();
            if (e.touches.length === 2) {
                const dist = getTouchDist(e.touches);
                if (lastTouchDist) {
                    scale *= dist / lastTouchDist;
                    lastTouchDist = dist;
                    scheduleDraw();
                }
            } else if (e.touches.length === 1) {
                offsetX += (e.touches[0].clientX - lastX) / scale;
                offsetY += (e.touches[0].clientY - lastY) / scale;
                lastX = e.touches[0].clientX;
                lastY = e.touches[0].clientY;
                scheduleDraw();
            }
        },
        { passive: false }
    );

    glcanvas.addEventListener('touchend', () => {
        if (event.touches && event.touches.length < 2) {
            lastTouchDist = null;
            scheduleDraw();
        }
    });

    function getTouchDist(touches) {
        const dx = touches[0].clientX - touches[1].clientX;
        const dy = touches[0].clientY - touches[1].clientY;
        return Math.sqrt(dx * dx + dy * dy);
    }
}