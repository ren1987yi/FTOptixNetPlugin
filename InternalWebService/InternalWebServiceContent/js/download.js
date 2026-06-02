window.onload = ()=>{

    const params = new URLSearchParams(window.location.search);
    const filename = params.get('f');
    const root = "./GetFileById";
    

  

    document.getElementById('downloadUrlBtn').addEventListener('click', async function () {

      

      postJson(root,JSON.stringify({Id:filename})).then(result=>{
        if(result != null && result != undefined){
            if(result.success){
                const fileUrl = result.url; // Replace with your file URL
                const fileName = result.name;

                const a = document.createElement('a');
                a.href = fileUrl;
                a.download = fileName; // Suggests a filename
                document.body.appendChild(a);
                a.click();
                document.body.removeChild(a);
            } else {
                alert("Failed to get file URL: " + result.message);
            }
        } else {
            alert("Failed to get file URL: No response from server");
        }
      }).catch(error=>{
        alert("Failed to get file URL: " + error);
      });




 
    });

}