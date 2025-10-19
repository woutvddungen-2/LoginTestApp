window.blazorExtensions = {
    ReadCookie: function (name) {
        const value = `; ${document.cookie}`;
        const parts = value.split(`; ${name}=`);
        if (parts.length === 2) return parts.pop().split(';').shift();
        return null;
    },
    DeleteCookie: function (name) {
        document.cookie = name + '=; Max-Age=0; path=/';
    }
};
