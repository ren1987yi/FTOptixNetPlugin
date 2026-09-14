(function () {
    var chart = null;
    var timer = null;
    var resizeFrame = 0;
    var loadApi = './load';
    var dataApi = './getdata';

    var defaultOption = {
        backgroundColor: 'transparent',
        tooltip: {
            trigger: 'axis'
        },
        grid: {
            left: 40,
            right: 24,
            top: 32,
            bottom: 36,
            containLabel: true
        },
        xAxis: {
            type: 'category',
            boundaryGap: false,
            data: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']
        },
        yAxis: {
            type: 'value'
        },
        series: [
            {
                name: 'Default',
                type: 'line',
                smooth: true,
                areaStyle: {},
                data: [120, 200, 150, 80, 70, 110, 130]
            }
        ]
    };

    function normalizeBackgroundColor(color) {
        if (!color) {
            return '#ffffff';
        }

        var value = color.trim();
        if (!value) {
            return '#ffffff';
        }

        if (/^[0-9a-fA-F]{3}$/.test(value) || /^[0-9a-fA-F]{6}$/.test(value) || /^[0-9a-fA-F]{8}$/.test(value)) {
            return '#' + value;
        }

        return value;
    }

    function getParams() {
        var params = new URLSearchParams(window.location.search);
        var period = parseInt(params.get('period') || '0', 10);

        if (isNaN(period)) {
            period = 0;
        }

        


        return {
            id: params.get('id') || '',
            bg: normalizeBackgroundColor(params.get('bg')),
            period: period
        };
    }

    function applyBackground(color) {
        document.documentElement.style.backgroundColor = color;
        document.body.style.backgroundColor = color;

        var chartElement = document.getElementById('chart');
        if (chartElement) {
            chartElement.style.backgroundColor = color;
        }
    }

    async function postJson(url, data) {
        var response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        });

        if (!response.ok) {
            throw new Error('Server error: ' + response.status + ' ' + response.statusText);
        }

        return await response.json();
    }

    function getResponseData(result) {
        if (!result || result.success !== true) {
            return null;
        }

        if (result.data !== undefined && result.data !== null) {
            return result.data;
        }

        return null;
    }

    function buildRequest(params) {
        return {
            id: params.id,
            clientid: params.id
        };
    }

    async function loadChart(params) {
        try {
            var result = await postJson(loadApi, buildRequest(params));
            var option = getResponseData(result);

            if (option) {
                chart.setOption(option, true);
                return;
            }

            console.warn('load failed:', result && result.message ? result.message : result);
        } catch (error) {
            console.warn('load error:', error);
        }

        chart.setOption(defaultOption, true);
    }

    async function updateChart(params) {
        try {
            var result = await postJson(dataApi, buildRequest(params));
            var option = getResponseData(result);

            if (option) {
                chart.setOption(option);
                return;
            }

            console.warn('getdata failed:', result && result.message ? result.message : result);
        } catch (error) {
            console.warn('getdata error:', error);
        }
    }

    function resizeChart() {
        if (resizeFrame) {
            window.cancelAnimationFrame(resizeFrame);
        }

        resizeFrame = window.requestAnimationFrame(function () {
            resizeFrame = 0;
            if (chart) {
                chart.resize();
            }
        });
    }

    async function init() {
        var params = getParams();
        applyBackground(params.bg);

        if (!window.echarts) {
            console.error('echarts is not loaded.');
            return;
        }

        chart = window.echarts.init(document.getElementById('chart'));
        window.addEventListener('resize', resizeChart);

        await loadChart(params);
        await updateChart(params);
        if (params.period > 0) {
            timer = window.setInterval(function () {
                updateChart(params);
            }, Math.max(params.period, 500));
        }
    }

    window.addEventListener('beforeunload', function () {
        if (timer) {
            window.clearInterval(timer);
            timer = null;
        }

        if (resizeFrame) {
            window.cancelAnimationFrame(resizeFrame);
            resizeFrame = 0;
        }

        if (chart) {
            chart.dispose();
            chart = null;
        }
    });

    window.addEventListener('DOMContentLoaded', init);
})();