// Static assets bundling/minification guidance is documented in the README.

(() => {
    const storageKey = "demo.sidebar.collapsed";
    const scrollKey = "demo.sidebar.scrollTop";
    const body = document.body;
    const isMobile = () => window.matchMedia("(max-width: 992px)").matches;

    const setCollapsed = (collapsed) => {
        if (isMobile()) {
            body.classList.toggle("sidebar-open", !collapsed);
            body.classList.remove("sidebar-collapsed");
            document.documentElement.classList.remove("sidebar-collapsed");
            return;
        }
        body.classList.toggle("sidebar-collapsed", collapsed);
        document.documentElement.classList.toggle("sidebar-collapsed", collapsed);
    };

    const restore = () => {
        const stored = localStorage.getItem(storageKey);
        if (stored !== null) {
            setCollapsed(stored === "true");
        }
    };

    const handleToggle = (event) => {
        event.preventDefault();
        if (isMobile()) {
            const open = body.classList.contains("sidebar-open");
            body.classList.toggle("sidebar-open", !open);
            return;
        }
        const collapsed = body.classList.contains("sidebar-collapsed");
        const next = !collapsed;
        setCollapsed(next);
        localStorage.setItem(storageKey, String(next));
    };

    document.addEventListener("click", (event) => {
        const target = event.target;
        if (!(target instanceof Element)) {
            return;
        }
        const toggle = target.closest("[data-sidebar-toggle]");
        if (!toggle) {
            return;
        }
        handleToggle(event);
    });

    const directToggle = document.getElementById("sidebarToggle");
    if (directToggle) {
        directToggle.addEventListener("click", handleToggle);
    }

    window.addEventListener("resize", () => {
        if (isMobile()) {
            body.classList.remove("sidebar-collapsed");
            document.documentElement.classList.remove("sidebar-collapsed");
        } else {
            body.classList.remove("sidebar-open");
            restore();
        }
    });

    restore();
    window.toggleSidebar = handleToggle;

    const sidebarNav = document.querySelector(".sidebar-nav");
    if (sidebarNav) {
        const saved = sessionStorage.getItem(scrollKey);
        if (saved) {
            sidebarNav.scrollTop = Number(saved);
        }
        window.addEventListener("beforeunload", () => {
            sessionStorage.setItem(scrollKey, String(sidebarNav.scrollTop));
        });
    }
})();
