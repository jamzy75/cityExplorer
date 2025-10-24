window.theme = {
    get() {
        return document.documentElement.getAttribute('data-theme') || 'dark';
    },
    set(n) {
        if (!n) n = 'dark';
        document.documentElement.setAttribute('data-theme', n);
        try { localStorage.setItem('theme', n); } catch { /* storage can fail in some modes */ }
    },
    toggle() {
        const cur = this.get();
        const n = (cur === 'light') ? 'dark' : 'light';
        this.set(n);
        return n;
    },
    init() {
        let saved = 'dark';
        try { saved = localStorage.getItem('theme') || 'dark'; } catch { /* ignore */ }
        this.set(saved);
    }
};


(function bootstrapTheme(){
    
    window.theme.init();
})();
