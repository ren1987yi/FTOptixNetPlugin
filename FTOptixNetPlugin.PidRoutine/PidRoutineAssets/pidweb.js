
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




var OPEN_FILEPATH = '';
var OPEN_AREA = 0;
var OPEN_STEP = '';

(async function () {
    // ===== 单一 WebGL Canvas，所有内容（线条/圆形/圆弧/矩形/多边形/文字）均由 GPU 渲染 =====


    const filepath = getQueryParam('f', '');
    const area = getQueryParam('a', '');
    const step = getQueryParam('s', '');




    OPEN_FILEPATH = filepath;
    OPEN_AREA = area;
    OPEN_STEP = step;

    await postData(`/app/open?f=${encodeURIComponent(filepath)}&a=${encodeURIComponent(area)}&s=${encodeURIComponent(step)}`, { f: filepath, a: area, s: step })
        .then(result => {

            console.log('成功:', result);
            drawPid(result);
        }
        )
        .catch(err => {
            console.error('失败:', err);
            drawPid({ Success: false, Data: {
                PolyLines: [],
                Texts: [],
                Circles: [],
                Arcs: [],
                Interactives: [],
                Height: 8000,
                width: 8000
            } });
        });
})();


















function drawPid(dataJson) {

    if (!dataJson.Success) {
        console.error('数据加载失败:', dataJson);
    }


    let data = dataJson.Data.page;

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
    // 着色器程序 1.5：高亮层专用“恒定像素宽度”粗线（用于折线/圆/圆弧/文字边框高亮）
    // 顶点数据为三角形（每段线用 2 个三角形构成一个矩形条带），
    // aPosition 为世界坐标上的基准点（与 lineVS 中的 aPosition 语义一致），
    // aNormal 为该点处线段的单位法线方向（已包含正负号，指向线条两侧）。
    // 关键点：法线偏移是在“乘以 uScale 之后”再叠加的（uHalfWidthPx 是屏幕像素单位），
    // 因此线宽在任意缩放级别下都保持恒定的屏幕像素宽度，不会随视图缩放变粗/变细，
    // 这一点与 shapeVS 的广告牌图形（局部偏移会随 uScale 缩放）不同。
    // ===================================================================
    const thickLineVS = `
    attribute vec2 aPosition;
    attribute vec2 aNormal;
    uniform vec2 uOffset;
    uniform float uScale;
    uniform vec2 uResolution;
    uniform float uHalfWidthPx;
    void main() {
      vec2 screenPos = (aPosition + uOffset) * uScale + aNormal * uHalfWidthPx;
      vec2 clip = vec2(
        screenPos.x / uResolution.x * 2.0 - 1.0,
        1.0 - screenPos.y / uResolution.y * 2.0
      );
      gl_Position = vec4(clip, 0.0, 1.0);
    }
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
    uniform float uRotation;
    varying vec2 vTexCoord;
    void main() {
      // 注意：屏幕/纹理局部坐标系是 y-down（向下为正），而 Rotate 角度沿用的是
      // CAD/DWG 常见的 y-up 数学约定（逆时针为正）。若直接用标准旋转矩阵，
      // 会导致视觉上出现镜像效果（表现为文字上下颠倒）。
      // 这里在 y-down 空间中对角度取反，等效于先把局部坐标转换到 y-up 空间
      // 旋转后再转换回来，从而与 CAD 的旋转方向保持一致。
      float c = cos(uRotation);
      float s = -sin(uRotation);
      vec2 rotated = vec2(
        aLocalOffset.x * c - aLocalOffset.y * s,
        aLocalOffset.x * s + aLocalOffset.y * c
      );
      vec2 screenPos = (uCenter + uOffset) * uScale + rotated * uScale;
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
        uRotation: gl.getUniformLocation(textProgram, 'uRotation'),
        uSampler: gl.getUniformLocation(textProgram, 'uSampler')
    };

    const thickLineProgram = createProgram(thickLineVS, solidFS);
    const thickLineLoc = {
        aPosition: gl.getAttribLocation(thickLineProgram, 'aPosition'),
        aNormal: gl.getAttribLocation(thickLineProgram, 'aNormal'),
        uOffset: gl.getUniformLocation(thickLineProgram, 'uOffset'),
        uScale: gl.getUniformLocation(thickLineProgram, 'uScale'),
        uResolution: gl.getUniformLocation(thickLineProgram, 'uResolution'),
        uHalfWidthPx: gl.getUniformLocation(thickLineProgram, 'uHalfWidthPx'),
        uColor: gl.getUniformLocation(thickLineProgram, 'uColor')
    };

    // ===================================================================
    // 着色器程序 1.6：点划线 / 虚线（CAD 线型），通过沿线累积世界坐标距离
    // 在片元着色器中按图案（dash-gap-dot-gap）丢弃像素来模拟线型。
    // uPattern = vec4(d1, d2, d3, d4)：图案内各段的“累积长度”边界（世界单位）：
    //   [0, d1)      = 画线段（dash）
    //   [d1, d2)     = 间隙
    //   [d2, d3)     = 点（dot，极短的画线段）
    //   [d3, d4)     = 间隙
    // 整个图案长度即为 d4，按 mod(距离, d4) 循环。
    // ===================================================================
    const dashLineVS = `
    attribute vec2 aPosition;
    attribute float aDist;
    uniform vec2 uOffset;
    uniform float uScale;
    uniform vec2 uResolution;
    varying float vDist;
    void main() {
      vec2 screenPos = (aPosition + uOffset) * uScale;
      vec2 clip = vec2(
        screenPos.x / uResolution.x * 2.0 - 1.0,
        1.0 - screenPos.y / uResolution.y * 2.0
      );
      gl_Position = vec4(clip, 0.0, 1.0);
      vDist = aDist;
    }
  `;
    const dashLineFS = `
    precision mediump float;
    uniform vec4 uColor;
    uniform vec4 uPattern; // d1, d2, d3, d4（世界单位下的累积边界）
    varying float vDist;
    void main() {
      float t = mod(vDist, uPattern.w);
      bool visible = (t < uPattern.x) || (t >= uPattern.y && t < uPattern.z);
      if (!visible) discard;
      gl_FragColor = uColor;
    }
  `;

    const dashLineProgram = createProgram(dashLineVS, dashLineFS);
    const dashLineLoc = {
        aPosition: gl.getAttribLocation(dashLineProgram, 'aPosition'),
        aDist: gl.getAttribLocation(dashLineProgram, 'aDist'),
        uOffset: gl.getUniformLocation(dashLineProgram, 'uOffset'),
        uScale: gl.getUniformLocation(dashLineProgram, 'uScale'),
        uResolution: gl.getUniformLocation(dashLineProgram, 'uResolution'),
        uColor: gl.getUniformLocation(dashLineProgram, 'uColor'),
        uPattern: gl.getUniformLocation(dashLineProgram, 'uPattern')
    };

    // 常见 CAD 线型的图案定义（单位：世界坐标下的“图案基本长度倍数”，
    // 实际使用时再乘以 patternScale 换算成世界单位，方便按需调整疏密）。
    // 数值含义：[dashLen, gap1Len, dotLen, gap2Len]
    const LINE_PATTERNS = {
        DASHED: [1, 1, 1, 1],       // 虚线：画-隙 循环（dot 长度为 0 时会被 gap1 覆盖，等效于普通虚线）
        DASHDOT: [2, 1, 1, 1],      // 点划线：画-隙-点-隙
        DASHDOTDOT: [2, 1, 1, 1],   // 双点划线（简化近似，如需精确双点可扩展 pattern 为更多段）
        CONTINUOUS: null            // 实线：不使用 dash 着色器，走普通 lineProgram
    };

    // 根据 pattern（[dash, gap1, dot, gap2]）与 patternScale（世界单位缩放）计算累积边界 uPattern
    function computePatternBoundaries(pattern, patternScale) {
        const s = patternScale || 1;
        const d1 = pattern[0] * s;
        const d2 = d1 + pattern[1] * s;
        const d3 = d2 + pattern[2] * s;
        const d4 = d3 + pattern[3] * s;
        return [d1, d2, d3, Math.max(d4, 0.0001)]; // 避免除零/mod(x,0)
    }

    // ===== 数据：按颜色分组的海量多段线，每组可独立设置颜色 =====
    // lineGroups: [{ color: [r,g,b,a], lines: [ [{x,y},...], ... ] }, ...]
    let lineGroups = generateDataPolylineGroups();


    // 将颜色归一化为字符串 key，用于分组
    function colorKey(c) {
        if (!c) return 'default';
        return c.join(',');
    }

    // 将颜色转换为 [r,g,b,a]（0~1 范围）。支持传入 [r,g,b] / [r,g,b,a]，取值范围 0~255 或 0~1
    function normalizeColor(c) {
        if (!c || !Array.isArray(c)) return LINE_COLOR.slice();
        const isByte = c.some(v => v > 1);
        const r = isByte ? c[0] / 255 : c[0];
        const g = isByte ? c[1] / 255 : c[1];
        const b = isByte ? c[2] / 255 : c[2];
        const a = c.length > 3 ? (isByte ? c[3] / 255 : c[3]) : 1.0;
        return [r, g, b, a];
    }

    function generateDataPolylineGroups() {
        const groupMap = new Map(); // key -> { color, lines }
        for (let i = 0; i < data.PolyLines.length; i++) {
            const p = data.PolyLines[i];
            const lineType = (p.LineType || 'CONTINUOUS').toUpperCase();
            if (lineType !== 'CONTINUOUS' && LINE_PATTERNS[lineType]) continue; // 非实线交给 dashLineGroupBuffers 处理

            const color = normalizeColor(p.Color);
            const key = colorKey(p.Color);
            const ltpoints = p.Points;

            let group = groupMap.get(key);
            if (!group) {
                group = { color, lines: [] };
                groupMap.set(key, group);
            }

            ltpoints.forEach(points => {
                group.lines.push(points);
            });
        }
        return Array.from(groupMap.values());
    }

    // 将 [r,g,b,a]（0~1）颜色转换为 css rgba() 字符串，供文字纹理绘制使用
    function colorToCssString(c) {
        const r = Math.round((c[0] ?? 1) * 255);
        const g = Math.round((c[1] ?? 1) * 255);
        const b = Math.round((c[2] ?? 1) * 255);
        const a = c.length > 3 ? c[3] : 1;
        return `rgba(${r},${g},${b},${a})`;
    }

    // 将 Position 字段统一解析为 { x, y }，兼容 [x,y] 数组或 {x,y} 对象两种格式
    function toXY(pos) {
        if (!pos) return { x: 0, y: 0 };
        if (Array.isArray(pos)) return { x: pos[0] || 0, y: pos[1] || 0 };
        return { x: pos.x || 0, y: pos.y || 0 };
    }

    // 按颜色分组解析 data.Texts，返回 [{ color, texts: [{ Value, Position, Height }, ...] }, ...]
    function generateDataTextGroups() {
        const groupMap = new Map(); // key -> { color, texts }
        if (!data.Texts || !Array.isArray(data.Texts)) return [];
        for (let i = 0; i < data.Texts.length; i++) {
            const t = data.Texts[i];
            const color = normalizeColor(t.Color);
            const key = colorKey(t.Color);

            let group = groupMap.get(key);
            if (!group) {
                group = { color, texts: [] };
                groupMap.set(key, group);
            }

            group.texts.push(t);
        }
        return Array.from(groupMap.values());
    }

    // 按颜色分组解析 data.Circles，返回 [{ color, circles: [{ Center, Radius }, ...] }, ...]
    function generateDataCircleGroups() {
        const groupMap = new Map(); // key -> { color, circles }
        if (!data.Circles || !Array.isArray(data.Circles)) return [];
        for (let i = 0; i < data.Circles.length; i++) {
            const c = data.Circles[i];
            const color = normalizeColor(c.Color);
            const key = colorKey(c.Color);

            let group = groupMap.get(key);
            if (!group) {
                group = { color, circles: [] };
                groupMap.set(key, group);
            }

            group.circles.push(c);
        }
        return Array.from(groupMap.values());
    }

    // 按颜色分组解析 data.Arcs，返回 [{ color, arcs: [{ Center, Radius, StartAngle, EndAngle }, ...] }, ...]
    // 注：StartAngle/EndAngle 假定已是弧度制（与 makeArcGeometry 保持一致），
    // 若后端实际为度数，只需在构建时乘以 Math.PI/180 转换即可。
    function generateDataArcGroups() {
        const groupMap = new Map(); // key -> { color, arcs }
        if (!data.Arcs || !Array.isArray(data.Arcs)) return [];
        for (let i = 0; i < data.Arcs.length; i++) {
            const a = data.Arcs[i];
            const color = normalizeColor(a.Color);
            const key = colorKey(a.Color);

            let group = groupMap.get(key);
            if (!group) {
                group = { color, arcs: [] };
                groupMap.set(key, group);
            }

            group.arcs.push(a);
        }
        return Array.from(groupMap.values());
    }





    // 每个分组一个 GL 缓冲区：{ buffer, vertexCount, color }
    function buildLineBuffer(lines) {
        let total = 0;
        for (let i = 0; i < lines.length; i++) total += Math.max(0, lines[i].length - 1);
        const bufData = new Float32Array(total * 4);
        let ptr = 0;
        for (let i = 0; i < lines.length; i++) {
            const pts = lines[i];
            for (let j = 1; j < pts.length; j++) {
                bufData[ptr++] = pts[j - 1].x;
                bufData[ptr++] = pts[j - 1].y;
                bufData[ptr++] = pts[j].x;
                bufData[ptr++] = pts[j].y;
            }
        }
        const buffer = gl.createBuffer();
        gl.bindBuffer(gl.ARRAY_BUFFER, buffer);
        gl.bufferData(gl.ARRAY_BUFFER, bufData, gl.STATIC_DRAW);
        return { buffer, vertexCount: total * 2 };
    }

    // 为每个颜色分组构建独立的顶点缓冲区
    const lineGroupBuffers = lineGroups.map(group => {
        const built = buildLineBuffer(group.lines);
        return { buffer: built.buffer, vertexCount: built.vertexCount, color: group.color };
    });

    // 与 buildLineBuffer 类似，但额外为每个顶点计算“沿该条折线累积的世界坐标距离”，
    // 供 dashLineFS 根据距离丢弃像素以形成虚线/点划线效果。
    // 注意：累积距离在每条独立折线（line）内部从 0 重新开始，
    // 因此每条线的线型图案都会从起点对齐，多条断续折线之间不会互相影响。
    function buildDashLineBuffer(lines) {
        let total = 0;
        for (let i = 0; i < lines.length; i++) total += Math.max(0, lines[i].length - 1);
        const bufData = new Float32Array(total * 6); // 每个顶点 (x, y, dist)，每段 2 个顶点
        let ptr = 0;
        for (let i = 0; i < lines.length; i++) {
            const pts = lines[i];
            let acc = 0;
            for (let j = 1; j < pts.length; j++) {
                const p0 = pts[j - 1];
                const p1 = pts[j];
                const dx = p1.x - p0.x;
                const dy = p1.y - p0.y;
                const segLen = Math.sqrt(dx * dx + dy * dy);

                bufData[ptr++] = p0.x;
                bufData[ptr++] = p0.y;
                bufData[ptr++] = acc;

                acc += segLen;

                bufData[ptr++] = p1.x;
                bufData[ptr++] = p1.y;
                bufData[ptr++] = acc;
            }
        }
        const buffer = gl.createBuffer();
        gl.bindBuffer(gl.ARRAY_BUFFER, buffer);
        gl.bufferData(gl.ARRAY_BUFFER, bufData, gl.STATIC_DRAW);
        return { buffer, vertexCount: total * 2 };
    }

    // 绘制一组点划线/虚线（gl.LINES 图元 + 片元丢弃实现线型）
    function drawDashLines(buffer, vertexCount, colorRGBA, patternBoundaries) {
        if (vertexCount === 0) return;
        gl.useProgram(dashLineProgram);
        gl.bindBuffer(gl.ARRAY_BUFFER, buffer);
        const stride = 3 * 4; // (x, y, dist) 每个 float 4 字节
        gl.enableVertexAttribArray(dashLineLoc.aPosition);
        gl.vertexAttribPointer(dashLineLoc.aPosition, 2, gl.FLOAT, false, stride, 0);
        gl.enableVertexAttribArray(dashLineLoc.aDist);
        gl.vertexAttribPointer(dashLineLoc.aDist, 1, gl.FLOAT, false, stride, 2 * 4);

        gl.uniform2f(dashLineLoc.uOffset, offsetX, offsetY);
        gl.uniform1f(dashLineLoc.uScale, scale);
        gl.uniform2f(dashLineLoc.uResolution, viewportCssWidth, viewportCssHeight);
        gl.uniform4fv(dashLineLoc.uColor, colorRGBA);
        gl.uniform4fv(dashLineLoc.uPattern, patternBoundaries);
        gl.drawArrays(gl.LINES, 0, vertexCount);
    }

    // 按“颜色 + 线型”联合分组：CAD 数据中通常每条 PolyLine 会带 LineType 字段
    // （如 'CONTINUOUS' / 'DASHED' / 'DASHDOT' / 'DASHDOTDOT'），
    // 实线走普通 lineGroupBuffers（gl.LINES 批量绘制，性能最优），
    // 非实线线型则单独分组，使用 dashLineProgram 绘制。
    function generateDataDashLineGroups() {
        const groupMap = new Map(); // key -> { color, lineType, lines }
        for (let i = 0; i < data.PolyLines.length; i++) {
            const p = data.PolyLines[i];
            const lineType = (p.LineType || 'CONTINUOUS').toUpperCase();
            if (lineType === 'CONTINUOUS' || !LINE_PATTERNS[lineType]) continue; // 实线/未知线型跳过，交给普通折线渲染

            const color = normalizeColor(p.Color);
            const key = colorKey(p.Color) + '|' + lineType;
            let group = groupMap.get(key);
            if (!group) {
                group = { color, lineType, lines: [] };
                groupMap.set(key, group);
            }
            p.Points.forEach(points => group.lines.push(points));
        }
        return Array.from(groupMap.values());
    }

    // 世界单位下的图案基础缩放：数值越大，虚线/点划线的画段和间隙看起来越长。
    // 可按实际图纸比例调整，或从数据里读取每条线专属的 LineTypeScale。
    const DASH_PATTERN_SCALE = 5;

    let dashLineGroups = generateDataDashLineGroups();
    const dashLineGroupBuffers = dashLineGroups.map(group => {
        const built = buildDashLineBuffer(group.lines);
        const pattern = LINE_PATTERNS[group.lineType] || LINE_PATTERNS.DASHDOT;
        return {
            buffer: built.buffer,
            vertexCount: built.vertexCount,
            color: group.color,
            patternBoundaries: computePatternBoundaries(pattern, DASH_PATTERN_SCALE)
        };
    });

    window.dashLineGroups = dashLineGroups;
    window.dashLineGroupBuffers = dashLineGroupBuffers;

    // 根据分组索引动态调整颜色（供外部调用，例如：setLineGroupColor(0, [1,0,0,1])）
    function setLineGroupColor(groupIndex, rgbaColor) {
        if (lineGroupBuffers[groupIndex]) {
            lineGroupBuffers[groupIndex].color = normalizeColor(rgbaColor);
            scheduleDraw();
        }
    }
    // 暴露到全局，便于在控制台/其他脚本中调用调试
    window.setLineGroupColor = setLineGroupColor;
    window.lineGroups = lineGroups;
    window.lineGroupBuffers = lineGroupBuffers;

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

    // 超采样基础倍数：文字位图按 fontPx * sample 的分辨率光栅化，
    // 但顶点几何仍按 fontPx 对应的逻辑尺寸生成（显示大小不变）。
    // 基础倍数结合设备像素比，保证静态时（未缩放）就足够清晰；
    // 最大倍数作为上限，避免文字过多时纹理过大影响性能与显存。
    const TEXT_SUPER_SAMPLE_BASE = Math.min(4, Math.max(2, Math.round((window.devicePixelRatio || 1) * 2)));
    const TEXT_SUPER_SAMPLE_MAX = 8;

    // 公共光栅化函数：将 text 按指定 sample 倍数光栅化到 offscreen canvas 上，
    // 返回位图实际尺寸 { w, h }，供 createTextTexture 与后续重新光栅化时复用。
    function rasterizeTextToOffscreen(text, fontPx, cssColor, sample) {
        const renderFontPx = fontPx * sample;

        offctx.font = `${renderFontPx}px sans-serif`;
        offctx.textBaseline = 'alphabetic'; // 保证 measureText 的 ascent/descent 相对基线一致
        const metrics = offctx.measureText(text);
        const padding = 4 * sample;

        // 优先使用文字的实际视觉高度（actualBoundingBoxAscent/Descent），
        // 这样带有较高/较低字符（如中文、带下伸的英文字母 g/y）时高度也是准确的；
        // 部分浏览器可能不支持这两个属性，此时回退到 renderFontPx*1.4 的经验估算值。
        const hasBoundingBox =
            typeof metrics.actualBoundingBoxAscent === 'number' &&
            typeof metrics.actualBoundingBoxDescent === 'number';
        const textHeight = hasBoundingBox
            ? metrics.actualBoundingBoxAscent + metrics.actualBoundingBoxDescent
            : renderFontPx * 1.4;
        // 实际视觉高度相对于绘制基线的上下偏移，用于按视觉包围盒精确居中绘制文字
        const ascent = hasBoundingBox ? metrics.actualBoundingBoxAscent : renderFontPx * 0.7;

        // 位图分辨率（超采样后）
        const w = Math.ceil(metrics.width) + padding * 2;
        const h = Math.ceil(textHeight) + padding * 2;

        offscreen.width = w;
        offscreen.height = h;
        // 重新设置尺寸后 context 状态会重置，需要重新设置字体
        offctx.font = `${renderFontPx}px sans-serif`;
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

        return { w, h };
    }

    function createTextTexture(text, fontPx, cssColor) {
        const sample = TEXT_SUPER_SAMPLE_BASE;
        const { w, h } = rasterizeTextToOffscreen(text, fontPx, cssColor, sample);

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
        // 显示尺寸按逻辑（未超采样）大小计算，保证视觉大小与之前一致，
        // 只是位图分辨率更高从而更清晰。
        const displayW = w / sample;
        const displayH = h / sample;
        const hw = displayW / 2;
        const hh = displayH / 2;
        const interleaved = new Float32Array([
            -hw, -hh, 0, 0,
            hw, -hh, 1, 0,
            hw, hh, 1, 1,
            -hw, hh, 0, 1
        ]);
        const buffer = createGLBuffer(interleaved);

        return {
            texture, buffer, vertexCount: 4, width: displayW, height: displayH,
            // 以下元数据用于在缩放时动态重新光栅化以提升清晰度（见 ensureTextLabelSharpness）
            _text: text, _fontPx: fontPx, _cssColor: cssColor, _sample: sample
        };
    }

    // 根据当前视图缩放级别，动态提升文字纹理的超采样倍数。
    // 因为文字是广告牌方式渲染（screenPos = (center+offset)*scale + localOffset*scale），
    // 局部偏移也会被 scale 放大，因此放大视图时文字在屏幕上的实际像素尺寸会变大，
    // 若纹理分辨率不足就会出现模糊。此函数在需要时重新光栅化同一个 texture，
    // 保持顶点 buffer（显示尺寸）不变，仅提升位图像素密度。
    // 为避免频繁重建，按 2 的幂次分档（1x/2x/4x/8x...）只在跨过档位时才升级。
    function ensureTextLabelSharpness(label) {
        if (!label || !label._text) return; // 非标准文字标签（无元数据）跳过
        const zoomFactor = Math.max(1, scale);
        const steps = Math.max(0, Math.ceil(Math.log2(zoomFactor)));
        const desiredSample = Math.min(
            TEXT_SUPER_SAMPLE_MAX,
            TEXT_SUPER_SAMPLE_BASE * Math.pow(2, steps)
        );
        if (desiredSample <= label._sample) return; // 当前分辨率已足够，无需重建

        rasterizeTextToOffscreen(label._text, label._fontPx, label._cssColor, desiredSample);
        gl.bindTexture(gl.TEXTURE_2D, label.texture);
        gl.pixelStorei(gl.UNPACK_FLIP_Y_WEBGL, false);
        gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, offscreen);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.LINEAR);
        label._sample = desiredSample;
    }

    function drawTextLabel(label, centerX, centerY, rotationRad) {
        ensureTextLabelSharpness(label);

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
        gl.uniform1f(textLoc.uRotation, rotationRad || 0);

        gl.drawArrays(gl.TRIANGLE_FAN, 0, label.vertexCount);
    }

    // ===================================================================
    // 高亮层（Highlight Layer）：支持对折线、圆、圆弧、文字等已渲染元素做高亮描边，
    // 统一使用固定的“屏幕像素宽度”粗线（默认宽度 3px、颜色绿色），且宽度不受视图缩放影响。
    //
    // 实现原理：每个高亮图形都被转换为一组世界坐标点（折线/圆/圆弧本身就是点序列；
    // 文字则用其外接矩形的 4 个角点，并按与文字渲染相同的旋转公式旋转），
    // 再用 buildThickLineQuadData 为每一段相邻点生成一个“沿法线方向外扩 uHalfWidthPx
    // 像素”的矩形（两个三角形），通过 thickLineProgram 绘制。
    // 由于法线偏移是在乘以 uScale 之后才叠加的（对应屏幕像素），故不论放大缩小，
    // 描边宽度看起来都恒定为设定的像素宽度，这正是它与 gl.lineWidth()（在大多数
    // 现代浏览器/ANGLE 后端下会被限制为 1px）相比的优势。
    // ===================================================================
    const HIGHLIGHT_COLOR = [0.0, 1.0, 0.0, 1.0]; // 绿色
    const HIGHLIGHT_LINE_WIDTH_PX = 5; // 线宽 3px（恒定屏幕像素宽度）

    let highlightItems = []; // [{ buffer, vertexCount, color, halfWidthPx }, ...]

    // 将一组世界坐标点（{x,y} 或 [x,y]）转换为“恒定像素宽度”粗线的三角形顶点数据。
    // 每个顶点：[x, y, nx, ny]（位置 + 法线，法线在着色器中乘以 uHalfWidthPx 得到像素偏移）。
    // closed=true 时首尾相连形成闭合环（用于圆形/文字边框），否则为开放折线（用于折线/圆弧）。
    function buildThickLineQuadData(points, closed) {
        const pts = points.map(p => (Array.isArray(p) ? { x: p[0], y: p[1] } : p));
        if (pts.length < 2) return new Float32Array(0);

        const segmentCount = closed ? pts.length : pts.length - 1;
        const verts = new Float32Array(segmentCount * 6 * 4); // 每段 2 个三角形 = 6 顶点，每顶点 4 个 float
        let ptr = 0;

        for (let i = 0; i < segmentCount; i++) {
            const p0 = pts[i];
            const p1 = pts[(i + 1) % pts.length];
            let dx = p1.x - p0.x;
            let dy = p1.y - p0.y;
            const len = Math.sqrt(dx * dx + dy * dy) || 1;
            const nx = -dy / len;
            const ny = dx / len;

            // 两个三角形拼成一个矩形条带：(p0+n, p0-n, p1+n) 与 (p0-n, p1-n, p1+n)
            const push = (p, sign) => {
                verts[ptr++] = p.x;
                verts[ptr++] = p.y;
                verts[ptr++] = nx * sign;
                verts[ptr++] = ny * sign;
            };
            push(p0, 1); push(p0, -1); push(p1, 1);
            push(p0, -1); push(p1, -1); push(p1, 1);
        }

        return verts;
    }

    // 绘制单个高亮图形（三角形网格 + 恒定像素宽度）
    function drawThickLine(buffer, vertexCount, colorRGBA, halfWidthPx) {
        if (vertexCount === 0) return;
        gl.useProgram(thickLineProgram);
        gl.bindBuffer(gl.ARRAY_BUFFER, buffer);
        const stride = 4 * 4; // 4 个 float 一组（position.xy + normal.xy），每个 float 4 字节
        gl.enableVertexAttribArray(thickLineLoc.aPosition);
        gl.vertexAttribPointer(thickLineLoc.aPosition, 2, gl.FLOAT, false, stride, 0);
        gl.enableVertexAttribArray(thickLineLoc.aNormal);
        gl.vertexAttribPointer(thickLineLoc.aNormal, 2, gl.FLOAT, false, stride, 2 * 4);

        gl.uniform2f(thickLineLoc.uOffset, offsetX, offsetY);
        gl.uniform1f(thickLineLoc.uScale, scale);
        gl.uniform2f(thickLineLoc.uResolution, viewportCssWidth, viewportCssHeight);
        gl.uniform1f(thickLineLoc.uHalfWidthPx, halfWidthPx);
        gl.uniform4fv(thickLineLoc.uColor, colorRGBA);
        gl.drawArrays(gl.TRIANGLES, 0, vertexCount);
    }

    // 通用入口：添加一条高亮折线（任意点序列，世界坐标）
    // points: [{x,y}, ...] 或 [[x,y], ...]；closed: 是否首尾闭合；
    // color/widthPx 可选，缺省为绿色 3px。
    function addHighlightPolyline(points, closed, color, widthPx) {
        if (!points || points.length < 2) return null;
        const data = buildThickLineQuadData(points, !!closed);
        const buffer = createGLBuffer(data);
        const item = {
            buffer,
            vertexCount: data.length / 4,
            color: normalizeColor(color || HIGHLIGHT_COLOR),
            halfWidthPx: (widthPx || HIGHLIGHT_LINE_WIDTH_PX) / 2
        };
        highlightItems.push(item);
        scheduleDraw();
        return item;
    }

    // 高亮一个圆形：center 为 {x,y}/[x,y]，radius 世界坐标半径
    function addHighlightCircle(center, radius, opts) {
        opts = opts || {};
        const { x, y } = toXY(center);
        const segments = opts.segments || 48;
        const pts = [];
        for (let i = 0; i < segments; i++) {
            const a = (i / segments) * Math.PI * 2;
            pts.push({ x: x + Math.cos(a) * radius, y: y + Math.sin(a) * radius });
        }
        return addHighlightPolyline(pts, true, opts.color, opts.widthPx);
    }

    // 高亮一段圆弧：startAngle/endAngle 为 CAD 角度语义（弧度，+X 轴为 0°，逆时针为正），
    // 内部会按与 arcGroupModels 相同的方式做回绕归一化 + y 轴镜像修正，确保与实际圆弧显示一致。
    function addHighlightArc(center, radius, startAngle, endAngle, opts) {
        opts = opts || {};
        const { x, y } = toXY(center);
        let rawStart = startAngle || 0;
        let rawEnd = endAngle || 0;
        if (rawEnd <= rawStart) rawEnd += Math.PI * 2;
        const rStart = -rawStart;
        const rEnd = -rawEnd;
        const segments = opts.segments || 32;
        const pts = [];
        for (let i = 0; i <= segments; i++) {
            const a = rStart + (rEnd - rStart) * (i / segments);
            pts.push({ x: x + Math.cos(a) * radius, y: y + Math.sin(a) * radius });
        }
        return addHighlightPolyline(pts, false, opts.color, opts.widthPx);
    }

    // 高亮一个文字标注：用其外接矩形（宽 width、高 height，围绕 centerX/centerY，
    // 按 rotationRad 旋转）作为描边框。旋转公式与 textVS 保持一致（s = -sin(rotation)），
    // 确保高亮框的朝向与实际渲染的文字视觉朝向完全吻合。
    function addHighlightTextBox(centerX, centerY, width, height, rotationRad, opts) {
        opts = opts || {};
        rotationRad = rotationRad || 0;
        const hw = width / 2;
        const hh = height / 2;
        const c = Math.cos(rotationRad);
        const s = -Math.sin(rotationRad);
        const localCorners = [[-hw, -hh], [hw, -hh], [hw, hh], [-hw, hh]];
        const pts = localCorners.map(([lx, ly]) => ({
            x: centerX + (lx * c - ly * s),
            y: centerY + (lx * s + ly * c)
        }));
        return addHighlightPolyline(pts, true, opts.color, opts.widthPx);
    }

    // 便捷入口：直接传入某个 textGroupModels 的 label 项（{x,y,rotationRad,label}）来高亮该文字
    function addHighlightForTextItem(item, opts) {
        if (!item || !item.label) return null;
        return addHighlightTextBox(item.x, item.y, item.label.width, item.label.height, item.rotationRad, opts);
    }

    // 清除所有高亮（释放 GL 缓冲区）
    function clearHighlights() {

     

        highlightItems.forEach(item => gl.deleteBuffer(item.buffer));
        highlightItems = [];
        scheduleDraw();
        draw();
    }


    async function clearAllActive(){
           elements.forEach(el => {
            
            el.active = false;
        });

         await postData(`/app/clearall`, { });
           
        



    }

    // 暴露到全局，便于在控制台/其他脚本中调用调试与联调
    window.addHighlightPolyline = addHighlightPolyline;
    window.addHighlightCircle = addHighlightCircle;
    window.addHighlightArc = addHighlightArc;
    window.addHighlightTextBox = addHighlightTextBox;
    window.addHighlightForTextItem = addHighlightForTextItem;
    window.clearHighlights = clearHighlights;
    window.highlightItems = highlightItems;
    window.clearAllActive = clearAllActive;
    


    // ===================================================================
    // 10 个可交互元素（圆形，可拖拽/点击切换状态），全部由 WebGL 绘制
    // ===================================================================
    const ELEMENT_RADIUS = 45;

    const ELEMENT_WIDTH = 10;
    const ELEMENT_HEIGHT = 10;
    // 命中测试时在矩形半宽/半高基础上额外增加的世界坐标容差，
    // 用于弥补交互元素尺寸过小、鼠标难以精确点中的问题（不影响实际绘制大小）。
    const ELEMENT_HIT_PADDING = 2;

    // const elementCircle = makeCircleGeometry(ELEMENT_RADIUS, 40);
    const elementCircle = makeRectGeometry(ELEMENT_WIDTH, ELEMENT_HEIGHT);


    const elementFillBuffer = createGLBuffer(elementCircle.fill);
    const elementStrokeBuffer = createGLBuffer(elementCircle.stroke);

    // const elements = generateInteractiveElements(10, WORLD_SIZE);
    const elements = generateInteractiveElements2();
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



    function generateInteractiveElements2() {
        const list = [];

        for (let i = 0; i < data.Interactives.length; i++) {
            const p = data.Interactives[i];
            // 字段名兼容处理：不同数据来源可能用 TextHeight/textHeight/Height 表示文字高度，
            // 统一取第一个存在的值，缺省回退到 40，避免因字段名不一致导致 fontPx 为 undefined
            // （undefined 会使 canvas.font 无效，进而造成文字/交互元素错位或不可见）。
            const fontPx = p.TextHeight || p.textHeight || p.Height || 40;
            list.push({
                id: p.Handle,
                x: p.Position.x,
                y: p.Position.y,
                colorActive: [1.0,1.0,1.0, 0.2],
                colorNormal: [1.0,1.0,1.0, 0.2],
                label: createTextTexture(p.Text, fontPx, '#ffffff'),
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

    // ===================================================================
    // 按颜色分组的文字标注（来自 data.Texts）
    // 每条文字：Value 文字内容，Position 位置，Height 文字高度（像素）
    // ===================================================================
    let textGroups = generateDataTextGroups();
    const textGroupModels = textGroups.map(group => {
        const cssColor = colorToCssString(group.color);
        const labels = group.texts.map(t => {
            const { x, y } = toXY(t.Position);
            const fontPx = t.Height || 40;
            const label = createTextTexture(String(t.Value != null ? t.Value : ''), fontPx, cssColor);
            
            const rotationRad = t.Rotate;
            return { label, x, y, rotationRad };
        });
        return { color: group.color, labels };
    });

    // ===================================================================
    // 按颜色分组的圆形（来自 data.Circles）
    // 每个圆形：Center 圆心位置，Radius 半径（世界坐标单位）
    // 与多段线/文字一样按颜色分组，每个圆形单独建模（半径不同无法共用几何），
    // 但同组共用同一颜色，可通过 setCircleGroupColor 整组调色。
    // ===================================================================
    let circleGroups = generateDataCircleGroups();
    const circleGroupModels = circleGroups.map(group => {
        const items = group.circles.map(c => {
            const { x, y } = toXY(c.Center);
            const radius = c.Radius || 0;
            const geom = makeCircleGeometry(radius, 40);
            const fillBuffer = createGLBuffer(geom.fill);
            const strokeBuffer = createGLBuffer(geom.stroke);
            return { geom, fillBuffer, strokeBuffer, x, y };
        });
        return { color: group.color, items };
    });
    function setCircleGroupColor(groupIndex, rgbaColor) {
        if (circleGroupModels[groupIndex]) {
            circleGroupModels[groupIndex].color = normalizeColor(rgbaColor);
            scheduleDraw();
        }
    }
    window.setCircleGroupColor = setCircleGroupColor;
    window.circleGroups = circleGroups;
    window.circleGroupModels = circleGroupModels;

    // ===================================================================
    // 按颜色分组的圆弧（来自 data.Arcs）
    // 每个圆弧：Center 圆心位置，Radius 半径，StartAngle/EndAngle 起止角（弧度）
    // ===================================================================
    let arcGroups = generateDataArcGroups();
    const arcGroupModels = arcGroups.map(group => {
        const items = group.arcs.map(a => {
            const { x, y } = toXY(a.Center);
            const radius = a.Radius || 0;

            // ---- CAD 圆弧角度语义处理 ----
            // 1) CAD 约定：角度以 +X 轴为 0°，按“逆时针（CCW）”从 StartAngle 扫到 EndAngle，
            //    若 EndAngle 数值上 <= StartAngle，代表圆弧跨越了 0°/360°分界线，
            //    需要把 EndAngle 加上 2π，使其数值上大于 StartAngle，
            //    这样线性插值 a: start -> end 才会按“递增”方式正确扫过整段圆弧（CCW、经过 0°绕回）。
            let rawStart = a.StartAngle || 0;
            let rawEnd = a.EndAngle || 0;
            if (rawEnd <= rawStart) {
                rawEnd += Math.PI * 2;
            }

            // 2) 坐标系修正：CAD 使用 y-up（向上为正）+ 逆时针为正方向；
            //    而本渲染器的世界坐标经过管线后 +y 对应屏幕向下（y-down），
            //    直接使用 cos/sin(a) 会让图形在视觉上沿 x 轴发生上下镜像。
            //    对角度取负号可以抵消这一镜像，使 CAD 中"上"的方向在屏幕上视觉呈现也是"上"。
            //    先完成第 1 步的 CCW 回绕归一化（在 CAD 角度空间中），再统一取负号转换到渲染角度空间。
            const startAngle = -rawStart;
            const endAngle = -rawEnd;

            const geom = makeArcGeometry(radius, startAngle, endAngle, 32);
            const fillBuffer = createGLBuffer(geom.fill);
            const strokeBuffer = createGLBuffer(geom.stroke);
            return { geom, fillBuffer, strokeBuffer, x, y };
        });
        return { color: group.color, items };
    });
    function setArcGroupColor(groupIndex, rgbaColor) {
        if (arcGroupModels[groupIndex]) {
            arcGroupModels[groupIndex].color = normalizeColor(rgbaColor);
            scheduleDraw();
        }
    }
    window.setArcGroupColor = setArcGroupColor;
    window.arcGroups = arcGroups;
    window.arcGroupModels = arcGroupModels;
    window.textGroups = textGroups;
    window.textGroupModels = textGroupModels;




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

    // ---------------------------------------------------------------
    // 高亮层示例：分别高亮“第一条折线 / 第一个圆 / 第一段圆弧 / 第一条文字”，
    // 演示如何对之前所有类型的元素进行高亮（默认绿色、3px 恒定像素宽度）。
    // 如不需要该演示效果，注释掉以下代码块即可；也可参考这里的调用方式，
    // 改为高亮任意其他数据项（例如根据用户点击选中的图元）。
    // 注意：必须放在 scheduleDraw 定义之后调用（addHighlight* 内部会触发 scheduleDraw），
    // 否则会因为 let 声明的暂存死区（TDZ）报错 "Cannot access 'drawScheduled' before initialization"。
    // ---------------------------------------------------------------

/*
    if (lineGroups[0] && lineGroups[0].lines[0]) {
        addHighlightPolyline(lineGroups[0].lines[0], false,[1,0,0,1], 5);
    }
    if (circleGroups[0] && circleGroups[0].circles[0]) {
        const c0 = circleGroups[0].circles[0];
        addHighlightCircle(c0.Center, c0.Radius || 0);
    }
    if (arcGroups[0] && arcGroups[0].arcs[0]) {
        const a0 = arcGroups[0].arcs[0];
        addHighlightArc(a0.Center, a0.Radius || 0, a0.StartAngle || 0, a0.EndAngle || 0);
    }
    if (textGroupModels[0] && textGroupModels[0].labels[0]) {
        addHighlightForTextItem(textGroupModels[0].labels[0]);
    }
*/



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
        // 之前用固定的 ELEMENT_RADIUS(45) 做圆形命中测试，但实际绘制的交互元素
        // 已改为 ELEMENT_WIDTH x ELEMENT_HEIGHT（10x10）的小矩形，两者尺寸严重不匹配，
        // 会导致点击距离可视图形很远的地方也被误判为命中（定位不准）。
        // 这里改为按矩形自身半宽/半高做命中测试，并额外加一点世界坐标下的容差 padding，
        // 方便点击这种很小的交互元素，同时不会明显偏离实际可视范围。
        const halfW = ELEMENT_WIDTH / 2 + ELEMENT_HIT_PADDING;
        const halfH = ELEMENT_HEIGHT / 2 + ELEMENT_HIT_PADDING;
        for (let i = elements.length - 1; i >= 0; i--) {
            const el = elements[i];
            const dx = Math.abs(worldX - el.x);
            const dy = Math.abs(worldY - el.y);
            if (dx <= halfW && dy <= halfH) return el;
        }
        return null;
    }

    // ===================================================================
    // 主绘制函数
    // ===================================================================
    function draw() {
        gl.clearColor(0, 0, 0, 1);
        gl.clear(gl.COLOR_BUFFER_BIT);

        // 1) 海量线条：world-space 真实缩放，按颜色分组分别 draw call

        gl.useProgram(lineProgram);
        gl.uniform2f(lineLoc.uOffset, offsetX, offsetY);
        gl.uniform1f(lineLoc.uScale, scale);
        gl.uniform2f(lineLoc.uResolution, viewportCssWidth, viewportCssHeight);

        for (let i = 0; i < lineGroupBuffers.length; i++) {
            const group = lineGroupBuffers[i];
            if (group.vertexCount === 0) continue;
            gl.bindBuffer(gl.ARRAY_BUFFER, group.buffer);
            gl.enableVertexAttribArray(lineLoc.aPosition);
            gl.vertexAttribPointer(lineLoc.aPosition, 2, gl.FLOAT, false, 0, 0);
            gl.uniform4fv(lineLoc.uColor, group.color);
            gl.drawArrays(gl.LINES, 0, group.vertexCount);
        }

        // 1.5) 点划线 / 虚线（CAD 非实线线型），按颜色+线型分组，
        // 通过片元丢弃模拟 dash-gap-dot-gap 图案
        for (let i = 0; i < dashLineGroupBuffers.length; i++) {
            const g = dashLineGroupBuffers[i];
            drawDashLines(g.buffer, g.vertexCount, g.color, g.patternBoundaries);
        }



        // 2) 示例几何图形（圆弧/矩形/多边形/圆形），广告牌方式渲染，尺寸恒定不随缩放变形
        // drawShapeBuffer(demoArcFillBuffer, demoArc.fillCount, gl.TRIANGLE_FAN, demoArcCenter.x, demoArcCenter.y, demoArcColor);
        // drawShapeBuffer(demoArcStrokeBuffer, demoArc.strokeCount, gl.LINE_STRIP, demoArcCenter.x, demoArcCenter.y, demoArcStrokeColor);
        // drawTextLabel(demoArcLabel, demoArcCenter.x, demoArcCenter.y + 280);

        // drawShapeBuffer(demoRectFillBuffer, demoRect.fillCount, gl.TRIANGLE_FAN, demoRectCenter.x, demoRectCenter.y, demoRectColor);
        // drawShapeBuffer(demoRectStrokeBuffer, demoRect.strokeCount, gl.LINE_LOOP, demoRectCenter.x, demoRectCenter.y, demoRectStrokeColor);
        // drawTextLabel(demoRectLabel, demoRectCenter.x, demoRectCenter.y + 220);

        // drawShapeBuffer(demoHexFillBuffer, demoHex.fillCount, gl.TRIANGLE_FAN, demoHexCenter.x, demoHexCenter.y, demoHexColor);
        // drawShapeBuffer(demoHexFillBuffer, demoHex.strokeCount, gl.LINE_LOOP, demoHexCenter.x, demoHexCenter.y, demoHexStrokeColor);


        // polylineModels.forEach(pp => {
        //     // drawShapeBuffer(pp.HexFillBuffer, pp.Hex.fillCount, gl.TRIANGLE_FAN, pp.HexCenter.x, pp.HexCenter.y, pp.HexColor);
        //     drawShapeBuffer(pp.HexFillBuffer, pp.Hex.strokeCount, gl.LINE_LOOP, pp.HexCenter.x, pp.HexCenter.y, pp.HexStrokeColor);

        // });

        // drawTextLabel(demoHexLabel, demoHexCenter.x, demoHexCenter.y + 280);

        // drawShapeBuffer(demoCircleFillBuffer, demoCircle.fillCount, gl.TRIANGLE_FAN, demoCircleCenter.x, demoCircleCenter.y, demoCircleColor);
        // drawShapeBuffer(demoCircleStrokeBuffer, demoCircle.strokeCount, gl.LINE_LOOP, demoCircleCenter.x, demoCircleCenter.y, demoCircleStrokeColor);
        // drawTextLabel(demoCircleLabel, demoCircleCenter.x, demoCircleCenter.y + 240);

        // drawTextLabel(demoTitle, demoOriginX + 1000, demoOriginY - 120);

        // 3) 圆形（按颜色分组，来自 data.Circles）
        circleGroupModels.forEach(group => {
            group.items.forEach(item => {
                // drawShapeBuffer(item.fillBuffer, item.geom.fillCount, gl.TRIANGLE_FAN, item.x, item.y, group.color);
                drawShapeBuffer(item.strokeBuffer, item.geom.strokeCount, gl.LINE_LOOP, item.x, item.y, group.color);
            });
        });

        // 4) 圆弧（按颜色分组，来自 data.Arcs）
        arcGroupModels.forEach(group => {
            group.items.forEach(item => {
                // drawShapeBuffer(item.fillBuffer, item.geom.fillCount, gl.TRIANGLE_FAN, item.x, item.y, group.color);
                drawShapeBuffer(item.strokeBuffer, item.geom.strokeCount, gl.LINE_STRIP, item.x, item.y, group.color);
            });
        });

        // 5) 文字标注（按颜色分组，来自 data.Texts，每个文字纹理颜色已在创建时按组颜色烧录，支持按 Rotate 属性旋转）
        textGroupModels.forEach(group => {
            group.labels.forEach(item => {
                drawTextLabel(item.label, item.x, item.y, item.rotationRad);
            });
        });

        // 6) 10 个可交互元素（圆形 + 描边 + 文字标签）
        for (const el of elements) {
            const isHover = el === hoverElement;
            const isSelected = el === selectedElement;
            const fillColor = el.active ? el.colorActive : el.colorNormal;
            const strokeColor = isSelected ? [1, 1, 1, 0] : isHover ? [1, 1, 0, 0.5] : [0.2, 0.2, 0.2, 0];

            // drawShapeBuffer(elementFillBuffer, elementCircle.fillCount, gl.TRIANGLE_FAN, el.x, el.y, fillColor);
            drawShapeBuffer(elementStrokeBuffer, elementCircle.strokeCount, gl.LINE_LOOP, el.x, el.y, strokeColor);
            drawTextLabel(el.label, el.x, el.y);
        }

        // 7) 高亮层（最后绘制，叠加在最上层）：折线/圆/圆弧/文字边框的高亮描边，
        // 统一使用恒定像素宽度（默认 3px）与颜色（默认绿色），不受视图缩放影响。
        highlightItems.forEach(item => {
            drawThickLine(item.buffer, item.vertexCount, item.color, item.halfWidthPx);
        });
    }



    function processHighlight(data){
        if(data.Success){
            clearHighlights();
            
            var p = data.Data;

            var color = p.Color || [0, 1, 0, 1];
            var widthPx = p.WidthPx || 3;

            if(p.PolyLines && p.PolyLines.length>0){
                p.PolyLines.forEach(points => {
                    addHighlightPolyline(points, false, color, widthPx);
                });
            }

            if(p.Circles && p.Circles.length>0){
                p.Circles.forEach(c => {
                    addHighlightCircle(c.Center, c.Radius || 0, {color, widthPx});
                });
            }


            if(p.Arcs && p.Arcs.length>0){
                p.Arcs.forEach(a => {
                    addHighlightArc(a.Center, a.Radius || 0, a.StartAngle || 0, a.EndAngle || 0, {color, widthPx});
                });
            }

            if(p.Texts && p.Texts.length>0){
                p.Texts.forEach(t => {
                    const { x, y } = toXY(t.Position);
                    const fontPx = t.Height || 40;
                    const rotationRad = t.Rotate;
                    addHighlightTextBox(x, y, t.Width, t.Height, rotationRad, {color, widthPx});
                });
            }











        }
    }

    // ===================================================================
    // 鼠标交互
    // ===================================================================
    glcanvas.addEventListener('mousedown', async (e) => {
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

        if(hit){

            const ids = [];

            for(let i=0;i<elements.length;i++){
                if(elements[i].active){
                    ids.push(elements[i].id);
                }
            }

            await postData(`/app/search`, { f: OPEN_FILEPATH, a: OPEN_AREA ,ids: ids,s:OPEN_STEP})
            .then(result => {

                console.log('保存成功:', result);
                processHighlight(result);
            }
            )
            .catch(err =>
                console.error('保存失败:', err)
            );

        }

        draw();

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














    let dd = {
        Success : true,
        Data : dataJson.Data.hl
    } ;

    processHighlight(dd);

    let openedIds = dataJson.Data.ids;

    elements.forEach(element => {
        element.active = openedIds.includes(element.id);
    });


}