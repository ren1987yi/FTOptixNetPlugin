var myCodeMirror;
var cliId = '';
var filePath = '';
var load_api_root = './load';
var save_api_root = './save';

window.onload = () => {


    const params = new URLSearchParams(window.location.search);
    cliId = params.get('id');
    fileName = params.get('f');

    var option = {
        value: "function myScript(){\nreturn 100;\n}\n",
        mode: "javascript",
        styleActiveLine: true,
        lineNumbers: true,
        lineWrapping: false,
        foldGutter: true,
        gutters: ["CodeMirror-linenumbers", "CodeMirror-foldgutter"]
    };

    myCodeMirror = CodeMirror.fromTextArea(document.getElementById("code"), {
        value: "function myScript(){return 100;}\n",
        mode: "javascript",
        styleActiveLine: true,
        lineNumbers: true,
        lineWrapping: false,
        foldGutter: true,
        gutters: ["CodeMirror-linenumbers", "CodeMirror-foldgutter"]
    });

    fullHeight();

    load();
}

window.onresize = () => {
    fullHeight();
}


function fullHeight() {
    let el = document.getElementsByClassName("CodeMirror");
    el[0].style.height = (window.innerHeight - 0) + "px";
}


function save() {
    var txt = myCodeMirror.getValue();
    console.log(txt);

    let url = save_api_root;
    postJson(url,JSON.stringify(
        {
            clientid: cliId,
            filename:fileName,
            content:txt
        }
    ))
    .then()
    .catch();

}

function load() {

    let url = load_api_root;

    postJson(url, JSON.stringify(
        {
            clientid: cliId,
            filename:fileName
        }
    )
    ).then(result => {
        if (result != null && result != undefined) {
            if (result.success) {
                myCodeMirror.setValue(result.content);
            } else {
                alert(result.message);
            }
        } else {
            alert("handler failed to get content");
        }
    }
    ).catch(error => {
        alert(" ajax error,failed to get content");
    }
    );
}