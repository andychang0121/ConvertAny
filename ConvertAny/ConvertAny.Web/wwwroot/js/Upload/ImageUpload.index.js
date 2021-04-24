const _sizeLimited = 1.5;
const dropZone = document.getElementById("dropbox");

(fc => {
    const _fileContainer = document.getElementById(fc);

    ["dragenter", "dragover"].forEach(eventName => {
        _fileContainer.addEventListener(eventName, function (e) {
            preventDefaults(e);
            const dt = e.dataTransfer;
            dt.effectAllowed = "none";
            dt.dropEffect = "none";
        }, false);
    });

})("fileContainer");

(s => {
    const _dropZone = document.getElementById(s);

    if (!_dropZone) return;

    ["dragenter", "dragover", "dragleave", "drop"].forEach(e => {
        _dropZone.addEventListener(e, preventDefaults, false);
    });

    ["dragenter", "dragover"].forEach(e => {
        _dropZone.addEventListener(e, function () {
            highlight(_dropZone);
        }, false);
    });

    ["dragleave", "drop"].forEach(e => {
        _dropZone.addEventListener(e, function () {
            unhighlight(_dropZone);
        }, false);
    });

    _dropZone.addEventListener("drop", function (e) {
        const dt = e.dataTransfer;
        const files = dt.files;
        setDropFilesToTable(files);
    });

})("dropbox");

((t, o) => {
    const _target = document.getElementById(t);
    const _result = document.getElementById(o);

    if (!_target || !_result) return;

    _target.addEventListener("click", function (e) {
        if (_result) {
            _result.click();
        };
        preventDefaults(e);
    }, false);

})("fileSelect", "fileElem");

((t, s) => {
    const _target = document.getElementById(t);
    const _result = document.getElementById(s);

    if (!_target || !_result || _result.length === 0) return;

    _target.addEventListener("click", function (e) {
        preventDefaults(e);
        uploadFilesHandler();
    }, false);
})("uploadFiles", "fileResult");

function highlight(o) {
    o.classList.add("highlight");
}

function unhighlight(o) {
    o.classList.remove("highlight");
}

function setUploadFiles(b) {
    const _o = document.getElementById("uploadFiles");
    _o.disabled = b;
}

function setDropFilesToTable(files) {
    removeFileResult("[data-fileResult]");
    setDescription("[data-description]", false);

    const setNode = function (o) {
        o.removeAttribute("data-fileResult-template");
        o.removeAttribute("style");
        o.setAttribute("data-fileResult", "");
        return o;
    }
    const setFileInfo = function (o, file) {
        const _name = o.querySelector("[data-file-name]");
        const _size = o.querySelector("[data-file-size]");
        _name.innerHTML = file.name;
        _size.innerHTML = bytesToSize(file.size);
        return o;
    }
    const setImage = function (o, src, file) {
        const _img = o.querySelector("[data-img]").getElementsByTagName("img");
        _img[0].src = src;
        _img[0].file = file;
        return o;
    }
    const _fileResult = document.getElementById("fileResult");
    const _template = document.querySelector("[data-fileResult-template]");

    [].forEach.call(files, (_file) => {
        getFileBase64(_file).then(function (r) {
            let _fileRow = setNode(_template.cloneNode(true));
            _fileRow.isUpload = validFileSize(_file.size, _sizeLimited);
            _fileRow = setFileInfo(_fileRow, _file);
            _fileRow = setImage(_fileRow, r, _file);

            setAlert(_fileRow, _fileRow.isUpload);
            _fileResult.appendChild(_fileRow);
        });
    });
    setUploadFiles(false);
}

function setAlert(o, isUpload) {
    const _alert = o.querySelector("[data-alert]");
    if (isUpload) {
        _alert.remove();
        return;
    }
    o.querySelector("[data-progress]").remove();
    _alert.removeAttribute("style");
    return;
}

function uploadFilesHandler() {
    const customSize = getCustomSize();
    const _fileRs = document.querySelectorAll("[data-fileResult]");

    [].forEach.call(_fileRs, function (_rs) {
        const _progress = _rs.querySelector("[data-progress]");
        const _image = _rs.getElementsByTagName("img")[0];
        const _file = _image.file;

        if (validFileSize(_file.size, _sizeLimited)) {
            const _request = {
                fileName: _file.name,
                size: _file.size,
                type: _file.type,
                base64: _image.src,
                customWidth: customSize.customWidth,
                customHeight: customSize.customHeight
            };
            getImage(_image.src).then(function (_r) {
                _request.width = _r.width;
                _request.height = _r.height;
            }).then(() => {
                _progress.style.display = "";

                const _progressbar = _progress.querySelector("[data-progress-bar]");
                const _requestToken = document.getElementsByName("__RequestVerificationToken");
                const _requestData = {
                    __RequestVerificationToken: _requestToken[0].value,
                    requestData: _request
                };

                postUploadFile("/upload/post", _request, _progressbar);
            });
        }
    });
}

function setAjaxLoader(b) {
    const _loader = document.getElementById("overlay");
    _loader.style.display = b ? "block" : "none";
}

function postUploadFile(url, data, progressbar) {
    $.post({
        xhr: function () {
            const xhr = new window.XMLHttpRequest();
            xhr.upload.addEventListener("progress", function (event) {
                if (event.lengthComputable) {
                    const percentComplete = (event.loaded / event.total) * 100;
                    const pc = (Math.round(percentComplete));
                    progressbar.style.width = `${pc}%`;
                }
            }, false);
            return xhr;
        },
        xhrFields: {
            onprogress: function (event) {
                if (event.lengthComputable) {
                    if (event.lengthComputable) {
                        const percentComplete = (event.loaded / event.total) * 100;
                        const pc = (Math.round(percentComplete));

                        progressbar.classList.remove("progress-bar-danger");
                        progressbar.style.width = progressbar.style.width === "100%"
                            ? "0%" : progressbar.style.width;

                        progressbar.style.width = pc + "%";
                    }
                }
            }
        },
        url: url,
        /*data: data,*/
        data: JSON.stringify(data),
        contentType: "application/json",
        beforeSend: function () {
            setAjaxLoader(true);
        },
        success: function (r, textStatus, jqXHR) {
            console.log(r, textStatus, jqXHR);
            const _rs = r.data;
            getDownloadFile(_rs.fileName, _rs.result).then(function (link) {
                link.remove();
            });
        },
        complete: function () {
            setAjaxLoader(false);
        }
    });
}

function removeFileResult(s) {
    const _fileResult = document.querySelectorAll(s);
    if (_fileResult === undefined || !_fileResult) return;
    [].forEach.call(_fileResult, (fs) => {
        fs.remove();
    });
    return;
}

function removeSelected(o) {
    const _o = function (_s) {
        return new window.Promise((resolve, reject) => {
            const _fileResult = getParentNodeBySelector(o.parentNode, _s);
            if (!_fileResult) reject();
            _fileResult.remove();
            return resolve();
        });
    }
    _o("[data-fileResult]").then(() => {
        getFileResult("[data-fileResult]");
        setUploadFiles(true);
    });
}

function getFileResult(t) {
    const _fileResults = document.querySelectorAll(t);
    const _isEmpty = _fileResults.length === 0;
    setDescription("[data-description]", !_isEmpty);
}

function setDescription(s, b) {
    const _description = document.querySelector(s);

    _description.style.display = b ? "block" : "none";
}