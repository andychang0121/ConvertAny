const _loader = document.getElementById("overlay");
const _bsProgress = document.getElementById("progress");
const API_ENDPOINT = "/upload/post";
const API_ENDPOINT_ASYNC = "/upload/postdataAsync";

const [fileSelect, fileElem, uploadFiles, customWidth, customHeight] = [
    document.getElementById("fileSelect")
    , document.getElementById("fileElem")
    , document.getElementById("uploadFiles")
    , document.getElementById("customWidth")
    , document.getElementById("customHeight")];

let _width = 0;
let _imageCount = 0;

fileElem.addEventListener("change", function () {
    _bsProgress.classList.value = "progress-bar progress-bar-striped progress-bar-animated";
});

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

    setProgress(true);
    setLoaderPromise(true, _loader).then(function () {
        uploadFilesBase64Async(imgs, customWidth.value, customHeight.value);
    });

}, false);

function getUploadFiles(imgs) {
    const rs = [];
    [].forEach.call(imgs, function (img) {
        const _isUpload = validFileSize(img.file.size);
        if (_isUpload) {
            rs.push(img);
        }
    });
    return rs;
}

function uploadFilesBase64Async(imgs, customWidth, customHeight) {
    const ajaxPost = function (image, _requestData, _url) {
        const _file = image.file;

        getBase64(_file).then(function (r) {
            _requestData.base64 = r;
        }).then(function () {
            _requestData.fileName = image.name;
            _requestData.type = _file.type;
            _requestData.size = _file.size;
            _requestData.fileName = _file.name;
            _requestData.width = image.naturalWidth;
            _requestData.height = image.naturalHeight;
        }).then(function () {
            jUploadFile(_url, _requestData);
        });
    };
    const _imgs = getUploadFiles(imgs);

    _imageCount = _imgs.length;

    for (let image of _imgs) {
        const _requestData = {
            customWidth: customWidth,
            customHeight: customHeight
        };

        if (image.hasUpload) {
            ajaxPost(image, _requestData, API_ENDPOINT);
        } else {
            image.classList.add("clip-svg");
        };
    }
}

function jUploadFile(url, data) {
    const _token = $("[name*='__RequestVerificationToken']").val();
    const _request = {
        __RequestVerificationToken: _token,
        requestData: data
    }

    $.post({
        url: url,
        data: JSON.stringify(data),
        contentType: "application/json",
        async: false,
        beforeSend: function () {
            _imageCount--;
            console.log("beforeSend:", _imageCount);
        },
        complete: function () {
            console.log("complete:", _imageCount);
            setLoader(false, _loader);
        },
        success: function (r) {
            if (r.isOk) {
                const _url = `/upload/get?tempdataKey=${r.data}`;
                window.open(_url, "_blank");
            }
        }
    }).done(function () {
        console.log("done:", _imageCount);
    });
}

function setLoaderPromise(b, o) {

    return new Promise((resolve, reject) => {
        const _loader = o === undefined ? document.getElementById("overlay") : o;
        _loader.style.display = b ? "block" : "none";
        resolve();
    });


}

function setLoader(b, o) {
    const _loader = o === undefined ? document.getElementById("overlay") : o;
    _loader.style.display = b ? "block" : "none";
}

function setProgress(b, o) {
    const _o = o === undefined ? document.getElementById("progress") : o;

    if (b) {
        _o.style.width = "";
        _o.classList.value = "progress-bar progress-bar-striped progress-bar-animated";
    } else {
        _o.classList.value = "progress-bar bg-info";
    }
}

function uploadFileHandler(requestData) {
    setProgress(true);
    setLoader(true);

    uploadFile("POST", "/upload/postdataAsync", requestData)(2000)
        .then(function (r) {
            if (r.IsOk) {
                const _url = `/upload/get?tempdataKey=${r.Data}`;
                window.open(_url, "_blank");
            }
        }).then(function () {
            setProgress(false);
            setLoader(false);
        });
}

function uploadFile(method, url, requestData) {
    return function (ms) {
        return new Promise(function (resolve, reject) {
            setTimeout(function () {
                const request = new XMLHttpRequest();
                request.upload.addEventListener("progress", updateProgress, false);
                request.addEventListener("load", completeHandler, false);
                request.addEventListener("error", errorHandler, false);
                request.addEventListener("abort", abortHandler, false);

                request.open(method, url, false);

                request.setRequestHeader("Cache-Control", "no-cache");
                //request.setRequestHeader('Content-type', 'application/json');
                request.setRequestHeader("X-Requested-With", "XMLHttpRequest");

                request.onreadystatechange = function (e) {
                    if (request.readyState === XMLHttpRequest.DONE) {
                        if (request.status === 200) {
                            resolve(JSON.parse(request.response));
                        } else {
                            reject(new Error(request.statusText));
                        }
                    }
                }
                request.send(JSON.stringify(requestData));
            }, ms);
        });
    }
}

function pageReload() {
    location.reload();
}

function updateProgress(event) {
    if (event.lengthComputable) {

        const percentComplete = (event.loaded / event.total) * 100;

        const pc = Math.round(percentComplete);

        for (let i = _width; i < pc; i++) {
            _bsProgress.style.width = i + 1 + "%";
            _bsProgress.innerHTML = `${i + 1}% (complete)`;
        }
        _width = pc;
    }
}

function completeHandler() { }

function errorHandler() { }

function abortHandler() { }

function getElement(config) {
    const ele = document.createElement(config.type);
    ele.setAttribute("class", config.className);
    if (config.name) {
        console.log(config.name);
        input.setAttribute("name", config.name);
    }

    ele.textContent = config.text;
    return ele;
}

function setHTMLTRImage(file, i) {
    const _config = {
        type: "button",
        className: "btn btn-default btn-sm",
        text: "Remove"
    };

    const tr = document.createElement("tr");

    //const input = getElement(_config);
    //tr.appendChild(getTd(input));
    tr.appendChild(getTd(i + 1));
    tr.appendChild(getTd(file.name));
    tr.appendChild(getTd(""));
    tr.appendChild(getTd(bytesToSize(file.size)));
    //tr.appendChild(getTd(file.size.numberFormat(0, ".", ",")));
    //----
    const td = document.createElement("td");
    const img = document.createElement("img");

    img.alt = file.name;
    img.src = file.src;
    img.file = file.file;
    img.hasUpload = validFileSize(file.size);

    img.onload = function () {
        window.URL.revokeObjectURL(this.src);
    }
    td.appendChild(img);
    tr.appendChild(td);
    return tr;
}

function getTh(v) {
    const th = document.createElement("th");
    th.innerHTML = v;
    return th;
}

function getTd(v) {
    const td = document.createElement("td");
    if (typeof v === "object") {
        td.appendChild(v);
        return td;
    } else if (typeof v === "string" || typeof v === "number") {
        const _span = document.createElement('span');
        _span.innerHTML = v;
        td.appendChild(_span);
    }
    return td;
}

function setFilesToTable(files) {
    if (!files.length) return;

    const setHTMLTableImage = function (_tableBody, _fileLists) {
        for (let i = 0; i < _fileLists.length; i++) {
            const file = _fileLists[i];
            const _tr = setHTMLTRImage(file, i);
            _tableBody.appendChild(_tr);
        }
    }

    const _uploadFiles = document.getElementById("uploadFiles");
    const _fileSelectResult = document.getElementById("fileSelectResult");
    const tableHead = _fileSelectResult.querySelector("thead");
    const tableBody = _fileSelectResult.querySelector("tbody");

    tableHead.innerHTML = "";
    tableBody.innerHTML = "";

    //const headTr = document.createElement("tr");

    //headTr.appendChild(getTh(""));
    //headTr.appendChild(getTh("#"));
    //headTr.appendChild(getTh("Name"));
    //headTr.appendChild(getTh("Size"));
    //headTr.appendChild(getTh("Preview"));

    //tableHead.appendChild(headTr);

    const filesCount = files.length;

    _uploadFiles.disabled = false;

    const fileLists = [];

    for (let i = 0; i < filesCount; i++) {

        const file = files[i];

        const fileObj = {
            idx: i,
            name: file.name,
            size: file.size,
            type: file.type.split("/").pop(),
            file: file,
            src: window.URL.createObjectURL(file)
        }

        fileLists.push(fileObj);
    }
    setHTMLTableImage(tableBody, fileLists);
}

function getBase64(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();

        reader.addEventListener("progress", updateProgress, false);

        reader.readAsDataURL(file);
        reader.onload = () => resolve(reader.result);
        reader.onerror = error => reject(error);
    });
}