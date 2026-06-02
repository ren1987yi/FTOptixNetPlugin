    //x_spreadsheet('#xspreadsheet');


        // import Spreadsheet from "x-data-spreadsheet";
// If you need to override the default options, you can set the override
// const options = {};
// new Spreadsheet('#x-spreadsheet-demo', options);


import * as utils from "/js/m_utils.js"


export const Init = async () =>{


const options = {
    mode: 'edit', // edit | read
    showToolbar: true,
    showGrid: true,
    showContextmenu: true,
    view: {
      height: () => document.documentElement.clientHeight,
      width: () => document.documentElement.clientWidth,
    },
    row: {
      len: 100,
      height: 25,
    },
    col: {
      len: 26,
      width: 100,
      indexWidth: 60,
      minWidth: 60,
    },
    style: {
      bgcolor: '#ffffff',
      align: 'left',
      valign: 'middle',
      textwrap: false,
      strike: false,
      underline: false,
      color: '#0a0a0a',
      font: {
        name: 'Helvetica',
        size: 14,
        bold: false,
        italic: false,
      },
    },
  };

  // const s = new Spreadsheet("#x-spreadsheet-demo")
  const s = x_spreadsheet('#x-spreadsheet-demo',options)
    .loadData({}) // load data
    .change(data => {
      // save data to db
    });

  // data validation
  s.validate()


    // var data1 = await utils.getJson("./assets/data1.json");
    // s.loadData(data1);

    // s.reRender();
    const params = new URLSearchParams(window.location.search);
    const filename = params.get('f');

    await onLoadData(s,filename);


    return s;

}


// s.addSheet("s1",true);
// s.addSheet("s2",true);

// s.reRender();




export const onLoadData = async function (s,filename){

  let body = {
    filename: filename
  };
  
  var d = await utils.postJson("/ExcelView/Load",JSON.stringify(body));
  console.log( JSON.stringify(d));

  s.loadData(d);
  s.reRender();

}




export const onSaveData = async function (s){
  let d = s.getData();

  d.forEach(sheet => {
    delete sheet.rows['len'];
    
  });
  let txt = JSON.stringify(d) ;
  let reuslt = await utils.postJson('/ExcelView/Save',txt);


  console.log( reuslt);
}
