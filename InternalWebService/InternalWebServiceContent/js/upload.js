

async function postBlob(url, blob, filename) {
    try {
        // Validate inputs
        if (!(blob instanceof Blob)) {
            throw new Error("Provided data is not a Blob.");
        }
        if (typeof url !== "string" || !url.startsWith("http")) {
            throw new Error("Invalid URL.");
        }

        // Option 1: Send as raw binary with correct Content-Type
        const response = await fetch(url, {
            method: "POST",
            headers: {
                "Content-Type": blob.type || "application/octet-stream",
                "X-Filename": filename || "file.bin" // Optional custom header
            },
            body: blob
        });

        if (!response.ok) {
            throw new Error(`Server error: ${response.status} ${response.statusText}`);
        }

        const result = await response.json(); // or text(), blob(), etc.
        console.log("Upload successful:", result);
        return result;

    } catch (error) {
        console.error("Upload failed:", error);
    }
}


async function post(url,data) {
    try{
         // Option 1: Send as raw binary with correct Content-Type
        const response = await fetch(url, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
              
            },
            body: data
        });

        if (!response.ok) {
            throw new Error(`Server error: ${response.status} ${response.statusText}`);
        }

        const result = await response.json(); // or text(), blob(), etc.
        console.log("post successful:", result);
        return result;
    }catch(error){
        alert("post failed:", error);
    }
}

window.onload = () => {

    const params = new URLSearchParams(window.location.search);
    const id = params.get('id');
    const acceptType = params.get('accept') || "*/*"; // Default to accepting all file types if not specified
    // var url = window.location.href;

    const url_upload = "./upload";
    const url_notify = "./uploadtrigger";
    const fileInput = document.getElementById('fileInput');

    fileInput.accept = acceptType; // Accept all file types, you can specify if needed

    // const root = "http://127.0.0.1:49002/file/upload";
    // const root = "./file/upload";
    // const rot 
    document.getElementById('uploadBtn').addEventListener('click', async function () {
        
        if(fileInput.files.length < 1 ){
            return;
        }
        const file = fileInput.files[0];
        // const blob = new Blob([file], { type: file.type }); // Create a Blob
        // if (!file) {
        //     alert("Please select a file first.");
        //     return;
        // }


        
        // var result  = await postBlob(root,blob,file.filename);

        // if(result != null && result != undefined){
        //     if(result.success){

        //         fileInput.value = ''
        //         var d = {
        //             Id:id,
        //             File: result.file
        //         };
        //         await post(root1,JSON.stringify(d));


        //     }else{
        //         alert("发送失败");
        //     }

        // }else{
        //     alert("发送失败");
        // }




        const formData = new FormData();
        formData.append("file", file);

        const xhr = new XMLHttpRequest();
        // xhr.open("POST", '/file/upload', true); // Change to your server upload endpoint
        xhr.open("POST", url_upload, true); // Change to your server upload endpoint

        // Progress tracking
        xhr.upload.onprogress = function (event) {
            if (event.lengthComputable) {
                const percent = (event.loaded / event.total) * 100;
                document.getElementById('progressBar').value = percent;
            }
        };

        // Success
        xhr.onload = async function () {
            if (xhr.status === 200) {
                document.getElementById('status').textContent = "Upload successful!";

                
                let result = JSON.parse(xhr.responseText);
               
                var d = {
                    Id:id,
                    File: result.file,
                    Name:result.name
                };

                await post(url_notify,JSON.stringify(d));

                fileInput.value = '';
            } else {
                document.getElementById('status').textContent = "Upload failed: " + xhr.statusText;
            }
        };

        // Error handling
        xhr.onerror = function () {
            document.getElementById('status').textContent = "An error occurred during upload.";
        };

        xhr.send(formData);
    });



}
