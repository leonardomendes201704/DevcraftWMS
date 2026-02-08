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

    const attachIndexHeaderActions = () => {
        const header = document.querySelector(".grid-header-actions");
        if (!header) {
            return;
        }

        const helpButtons = Array.from(document.querySelectorAll("button[data-bs-toggle=\"modal\"]"))
            .filter(btn => {
                const text = btn.textContent?.trim().toLowerCase();
                return text === "help" || text === "ajuda";
            });
        const filterButtons = Array.from(document.querySelectorAll("[data-grid-action=\"filters\"]"));

        const actions = [...helpButtons, ...filterButtons];
        actions.forEach(btn => {
            if (btn instanceof HTMLElement) {
                header.insertBefore(btn, header.firstChild);
                const parent = btn.parentElement;
                if (parent && parent.classList.contains("filter-drawer-button") && parent.children.length === 0) {
                    parent.remove();
                }
                if (parent && parent.classList.contains("index-actions") && parent.children.length === 0) {
                    parent.remove();
                }
            }
        });
    };

    const attachSectionWarehouseCascade = () => {
        const warehouseSelect = document.querySelector("[data-section-warehouse]");
        const sectorSelect = document.querySelector("[data-section-sector]");

        if (!(warehouseSelect instanceof HTMLSelectElement) || !(sectorSelect instanceof HTMLSelectElement)) {
            return;
        }

        const buildOption = (value, text, selected) => {
            const option = document.createElement("option");
            option.value = value;
            option.textContent = text;
            if (selected) {
                option.selected = true;
            }
            return option;
        };

        const loadSectors = async (warehouseId) => {
            sectorSelect.innerHTML = "";
            sectorSelect.append(buildOption("", "Loading...", true));
            sectorSelect.disabled = true;

            if (!warehouseId) {
                sectorSelect.innerHTML = "";
                sectorSelect.append(buildOption("", "Select", true));
                sectorSelect.disabled = false;
                return;
            }

            try {
                const response = await fetch(`/Sections/SectorOptions?warehouseId=${encodeURIComponent(warehouseId)}`);
                if (!response.ok) {
                    throw new Error("Failed to load sectors.");
                }

                const data = await response.json();
                sectorSelect.innerHTML = "";
                sectorSelect.append(buildOption("", data.length ? "Select" : "No sectors found", true));
                data.forEach(item => {
                    sectorSelect.append(buildOption(item.value, item.text, false));
                });
            } catch (error) {
                sectorSelect.innerHTML = "";
                sectorSelect.append(buildOption("", "Failed to load sectors", true));
            } finally {
                sectorSelect.disabled = false;
            }
        };

        warehouseSelect.addEventListener("change", (event) => {
            const target = event.target;
            if (!(target instanceof HTMLSelectElement)) {
                return;
            }
            loadSectors(target.value);
        });
    };

    const attachSectionFilterCascade = () => {
        const warehouseSelect = document.querySelector(".js-section-warehouse-filter");
        const sectorSelect = document.querySelector(".js-section-sector-filter");

        if (!(warehouseSelect instanceof HTMLSelectElement) || !(sectorSelect instanceof HTMLSelectElement)) {
            return;
        }

        const buildOption = (value, text, selected) => {
            const option = document.createElement("option");
            option.value = value;
            option.textContent = text;
            if (selected) {
                option.selected = true;
            }
            return option;
        };

        const loadSectors = async (warehouseId) => {
            sectorSelect.innerHTML = "";
            sectorSelect.append(buildOption("", "Loading...", true));
            sectorSelect.disabled = true;

            if (!warehouseId) {
                sectorSelect.innerHTML = "";
                sectorSelect.append(buildOption("", "Select sector", true));
                sectorSelect.disabled = false;
                return;
            }

            try {
                const response = await fetch(`/Sections/SectorOptions?warehouseId=${encodeURIComponent(warehouseId)}`);
                if (!response.ok) {
                    throw new Error("Failed to load sectors.");
                }

                const data = await response.json();
                sectorSelect.innerHTML = "";
                sectorSelect.append(buildOption("", data.length ? "Select sector" : "No sectors found", true));
                data.forEach(item => {
                    sectorSelect.append(buildOption(item.value, item.text, false));
                });
            } catch (error) {
                sectorSelect.innerHTML = "";
                sectorSelect.append(buildOption("", "Failed to load sectors", true));
            } finally {
                sectorSelect.disabled = false;
            }
        };

        warehouseSelect.addEventListener("change", (event) => {
            const target = event.target;
            if (!(target instanceof HTMLSelectElement)) {
                return;
            }
            loadSectors(target.value);
        });
    };

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", () => {
            attachIndexHeaderActions();
            attachSectionWarehouseCascade();
            attachSectionFilterCascade();
        });
    } else {
        attachIndexHeaderActions();
        attachSectionWarehouseCascade();
        attachSectionFilterCascade();
    }
})();
