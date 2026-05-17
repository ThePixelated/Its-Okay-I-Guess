mergeInto(LibraryManager.library, {
    IsMobileBrowser: function () {
        // Cek user agent browser
        return /iPhone|iPad|iPod|Android/i.test(navigator.userAgent);
    },
});