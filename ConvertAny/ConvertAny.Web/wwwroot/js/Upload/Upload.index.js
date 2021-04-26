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

})("selectcustomfile", "sizefileElem");

((s) => {
    const _o = document.getElementById(s);
    _o.addEventListener("click", function () {
        resetCustomizeUploadFile();
    }, false);

})("resetCustomize");

((s, ts) => {
    const setToggleClass = function (p, b) {
        const _i = p.getElementsByTagName("i")[0];
        _i.classList = "fas";
        const _classItem = b ? "fa-chevron-circle-up" : "fa-chevron-circle-down";
        _i.classList.add(_classItem);
    }
    const _o = document.getElementById(s);
    _o.addEventListener("click", function () {
        const _rs = toggleCustomSize(ts);
        _o.dataset.customsize = _rs;

        setToggleClass(_o, _rs);

    }, false);

})("setCustomSize", ["customSizes", "customSizeFiles"]);

function toggleCustomSize(ts) {
    let _isHidden;
    ts.forEach(s => {
        const _o = document.getElementById(s);
        _isHidden = _o.style.display === "none";
        _o.style.display = _isHidden ? "" : "none";
    });
    return _isHidden;
}

function highlight(o) {
    o.classList.add("highlight");
}

function unhighlight(o) {
    o.classList.remove("highlight");
}

function resetCustomizeUploadFile() {
    const _i = document.getElementById("sizefileElem");
    const _display = document.getElementById("uploadcustomizefile");
    _i.value = "";
    _display.value = "";
    const _dataset = _display.dataset;
    for (let key in _dataset) {
        if (Object.prototype.hasOwnProperty.call(_dataset, key)) {
            _display.removeAttribute(`data-${key.split(/(?=[A-Z])/).join("-").toLowerCase()}`);
        }
    }
}

function setCustomizeFile(o) {
    const _f = o ? o[0] : undefined;
    if (!_f || _f.length === 0) return;
    const _name = _f.name;
    const _input = document.getElementById("uploadcustomizefile");
    _input.value = _name;
    Array.from(o).forEach(file => {
        _input.dataset.name = file.name;
        _input.dataset.size = file.size;
        _input.dataset.type = file.type;
        getFileBase64(file).then(r => {
            _input.dataset.base64 = r;
        });
    });
}

function setUploadFiles(b) {
    const _o = document.getElementById("uploadFiles");
    _o.disabled = b;

    const _i = _o.getElementsByTagName("i")[0];

    if (_i) {
        _i.classList.remove("fa-spin");
        _i.style.color = "";
        if (!b) {
            _i.classList.add("fa-spin");
            _i.style.color = "red";
        }
    }
}

function setDropFilesToTable(files) {
    const _filesLength = files.length;

    if (_filesLength === 0) return;

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

    const setRemoveAction = function (o) {
        const _dataRemove = o.querySelectorAll("[data-remove]")[0];
        _dataRemove.addEventListener("click", function (e) {
            preventDefaults(e);
            removeSelected(o);
        }, false);
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

            setRemoveAction(_fileRow);

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

                const _jsonData = JSON.stringify(_request);

                postUploadFile("/upload/post", _jsonData, _progressbar);
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

            xhr.addEventListener("progress", function (event) {
                console.log(event);
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
        data: data,
        contentType: "application/json",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("RequestVerificationToken", getRequestVerificationToken());
            setAjaxLoader(true);
        },
        success: function (r, textStatus, jqXHR) {
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
    o.remove();
    getFileResult("[data-fileResult]");
    setUploadFiles(true);

    //const _o = function (_s) {
    //    return new window.Promise((resolve, reject) => {
    //        const _fileResult = getParentNodeBySelector(o.parentNode, _s);
    //        if (!_fileResult) {
    //            reject();
    //        }

    //        return resolve(_fileResult);
    //    });
    //}
    //_o("[data-fileResult]").then((fr) => {
    //    fr.remove();
    //    getFileResult("[data-fileResult]");
    //    setUploadFiles(true);
    //});
}

function getFileResult(t) {
    const _fileResults = document.querySelectorAll(t);
    const _isEmpty = _fileResults.length === 0;
    setDescription("[data-description]", _isEmpty);
}

function setDescription(s, b) {
    const _description = document.querySelector(s);

    _description.style.display = b ? "block" : "none";
}