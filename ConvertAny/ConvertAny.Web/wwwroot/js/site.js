// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
Number.prototype.numberFormat = function (c, d, t) {
    var n = this,
        c = isNaN(c = Math.abs(c)) ? 2 : c,
        d = d == undefined ? "." : d,
        t = t == undefined ? "," : t,
        s = n < 0 ? "-" : "",
        i = String(parseInt(n = Math.abs(Number(n) || 0).toFixed(c))),
        j = (j = i.length) > 3 ? j % 3 : 0;
    return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
};

function bytesToSize(bytes) {
    if (bytes === 0) return "0 B";
    const k = 1024;
    const sizes = ["B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"];
    const i = Math.floor(Math.log(bytes) / Math.log(k));

    const rs = (bytes / Math.pow(k, i)).toPrecision(3) + " " + sizes[i];
    return rs;
}

function validFileSize(bytes) {
    if (bytes === 0) return true;
    const _getSize = function (_bytes) {
        return _bytes / 1024 / 1024;
    }
    return _getSize(bytes) <= 2;
}

document.body.addEventListener("touchstart", function (e) { e.preventDefault(); });
document.body.addEventListener("touchmove", function (e) { e.preventDefault(); });

Number.prototype.numberFormat = function (c, d, t) {
    var n = this,
        c = isNaN(c = Math.abs(c)) ? 2 : c,
        d = d == undefined ? "." : d,
        t = t == undefined ? "," : t,
        s = n < 0 ? "-" : "",
        i = String(parseInt(n = Math.abs(Number(n) || 0).toFixed(c))),
        j = (j = i.length) > 3 ? j % 3 : 0;
    return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
};

function getFileBase64(file) {
    return new window.Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = () => resolve(reader.result);
        reader.onerror = error => reject(error);
    });
}

function getImage(src) {
    return new window.Promise((resolve, revoke) => {
        const img = new Image();
        img.onload = () => resolve(img);
        img.crossOrigin = "Anonymous";
        img.src = src;
    });
}

function getDownloadFile(fileName, blob, type) {
    return new window.Promise((resolve, reject) => {
        const _type = type === undefined ? "application/zip" : type;
        const binaryString = window.atob(blob);
        const binaryLen = binaryString.length;
        const bytes = new Uint8Array(binaryLen);

        for (let i = 0; i < binaryLen; i++) {
            const ascii = binaryString.charCodeAt(i);
            bytes[i] = ascii;
        }

        const url = window.URL.createObjectURL(new Blob([bytes], { type: _type }));
        const link = document.createElement("a");
        link.href = url;
        link.setAttribute("download", fileName);
        document.body.appendChild(link);
        link.click();
        resolve(link);
    });
}

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

function getCustomSize() {
    const rs = {
        customWidth: document.getElementById("customWidth").value,
        customHeight: document.getElementById("customHeight").value
    }
    return rs;
}

function getParentNode(o, name) {
    const _tagName = o.tagName.toLowerCase();
    if (_tagName === name) return o;
    return getParentNode(o.parentNode, name);
}

function findArray(a, v) {
    if (!Array.isArray(a)) return false;
    return a.indexOf(v) >= 0;
}

function getParentNodeBySelector(o, name) {
    name = name.replace("[", "").replace("]", "").replace("data", "").replace("-", "").toLowerCase();
    if (o === null || o === undefined || o.dataset === undefined) return undefined;
    const _dataSet = Object.keys(o.dataset);
    if (findArray(_dataSet, name)) return o;
    return getParentNodeBySelector(o.parentNode, name);
}