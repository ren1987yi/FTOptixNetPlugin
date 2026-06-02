export const postBlob = async function (url, blob, filename) {
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


export const postJson = async function (url, data) {
    try {
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
    } catch (error) {
        alert("post failed:", error);
    }
}







export const getJson = async function (url) {


    try {
        // Option 1: Send as raw binary with correct Content-Type
        const response = await fetch(url, {
            method: "GET",
        });

        if (!response.ok) {
            throw new Error(`Server error: ${response.status} ${response.statusText}`);
        }

        const result = await response.json(); // or text(), blob(), etc.
        console.log("post successful:", result);
        return result;
    } catch (error) {
        alert("post failed:", error);
    }



}