const [fileSelect, fileElem, uploadFiles] = [
    document.getElementById("fileSelect")
    , document.getElementById("fileElem")
    , document.getElementById("uploadFiles")
];

fileSelect.addEventListener("click", function (e) {
    if (fileElem) {
        fileElem.click();
    };
    e.preventDefault();
}, false);

uploadFiles.addEventListener("click", function (e) {
    e.preventDefault();
    const fileSelectResult = document.getElementById("fileSelectResult");
    const tableBody = fileSelectResult.querySelector("tbody");
    const imgs = tableBody.getElementsByTagName("img");

    const isPortaitList = [];

    const form = new FormData();

    for (let image of imgs) {
        const isPortait = image.clientHeight > image.clientWidth;
        form.append("file", image.file);
        isPortaitList.push(isPortait);
    }

    form.append("isPortaits", isPortaitList);

    FileUpload(form);

}, false);




function FileUpload(form) {
    const API_ENDPOINT = "api/upload/post";
    const request = new XMLHttpRequest();
    request.open("POST", API_ENDPOINT, true);
    request.onreadystatechange = () => {
        if (request.readyState === 4 && request.status === 200) {
            console.log(JSON.parse(request.responseText), request);
            window.open("/api/upload/get", "_blank");
        }
    };
    request.send(form);
}

function handleFiles(files) {
    const getTh = function (v) {
        const th = document.createElement("th");
        th.innerHTML = v;
        return th;
    }

    const getTd = function (v) {
        const td = document.createElement("td");
        td.innerHTML = v;
        return td;
    }

    if (!files.length) {
        return;
    }

    const fileSelectResult = document.getElementById("fileSelectResult");
    const tableHead = fileSelectResult.querySelector("thead");
    const tableBody = fileSelectResult.querySelector("tbody");
    tableHead.innerHTML = "";
    tableBody.innerHTML = "";

    const headTr = document.createElement("tr");

    headTr.appendChild(getTh("#"));
    headTr.appendChild(getTh("Name"));
    headTr.appendChild(getTh("Size"));
    headTr.appendChild(getTh("Preview"));

    const filesCount = files.length;

    const fileLists = [];
    for (let i = 0; i < filesCount; i++) {
        const file = files[i];
        const fileObj = {
            idx: i,
            name: file.name,
            size: file.size,
            type: file.type.split('/').pop(),
            src: window.URL.createObjectURL(file),
            file: file
        }
        fileLists.push(fileObj);
    }

    for (let i = 0; i < fileLists.length; i++) {
        const file = fileLists[i];

        //const input = document.createElement("input");
        //input.setAttribute("type", "checkbox");
        //console.log(input);

        const tr = document.createElement("tr");

        tr.appendChild(getTd(i + 1));
        tr.appendChild(getTd(file.name));
        tr.appendChild(getTd(file.size.numberFormat(0, '.', ',')));

        const td = document.createElement("td");

        const img = document.createElement("img");
        img.src = file.src;
        img.file = file.file;
        img.onload = function () {
            window.URL.revokeObjectURL(this.src);
        }
        td.appendChild(img);
        tr.appendChild(td);

        tableBody.appendChild(tr);
    }
}

function getOrientation(file, callback) {
    const reader = new FileReader();
    reader.onload = function (e) {

        const view = new DataView(e.target.result);

        if (view.getUint16(0, false) !== 0xFFD8) {
            return callback(-2);
        }
        const length = view.byteLength;

        let offset = 2;

        while (offset < length) {
            if (view.getUint16(offset + 2, false) <= 8) return callback(-1);

            const marker = view.getUint16(offset, false);

            offset += 2;
            if (marker === 0xFFE1) {
                if (view.getUint32(offset += 2, false) !== 0x45786966) {
                    return callback(-1);
                }

                const little = view.getUint16(offset += 6, false) === 0x4949;
                offset += view.getUint32(offset + 4, little);
                const tags = view.getUint16(offset, little);
                offset += 2;
                for (let i = 0; i < tags; i++) {
                    if (view.getUint16(offset + (i * 12), little) === 0x0112) {
                        return callback(view.getUint16(offset + (i * 12) + 8, little));
                    }
                }
            }
            else if ((marker & 0xFF00) !== 0xFF00) {
                break;
            }
            else {
                offset += view.getUint16(offset, false);
            }
        }
        return callback(-1);
    };
    reader.readAsArrayBuffer(file);
}