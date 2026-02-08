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
            sectorSelect.value = "";
            loadSectors(target.value);
        });
    };

    const attachStructureWarehouseCascade = () => {
        const warehouseSelect = document.querySelector("[data-structure-warehouse]");
        const sectorSelect = document.querySelector("[data-structure-sector]");
        const sectionSelect = document.querySelector("[data-structure-section]");

        if (!(warehouseSelect instanceof HTMLSelectElement)
            || !(sectorSelect instanceof HTMLSelectElement)
            || !(sectionSelect instanceof HTMLSelectElement)) {
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

        const resetSectionSelect = (placeholder) => {
            sectionSelect.innerHTML = "";
            sectionSelect.append(buildOption("", placeholder, true));
        };

        const loadSectors = async (warehouseId) => {
            sectorSelect.innerHTML = "";
            sectorSelect.append(buildOption("", "Loading...", true));
            sectorSelect.disabled = true;
            resetSectionSelect("Select section");

            if (!warehouseId) {
                sectorSelect.innerHTML = "";
                sectorSelect.append(buildOption("", "Select sector", true));
                sectorSelect.disabled = false;
                return;
            }

            try {
                const response = await fetch(`/Structures/SectorOptions?warehouseId=${encodeURIComponent(warehouseId)}`);
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

        const loadSections = async (sectorId) => {
            resetSectionSelect("Loading...");
            sectionSelect.disabled = true;

            if (!sectorId) {
                resetSectionSelect("Select section");
                sectionSelect.disabled = false;
                return;
            }

            try {
                const response = await fetch(`/Structures/SectionOptions?sectorId=${encodeURIComponent(sectorId)}`);
                if (!response.ok) {
                    throw new Error("Failed to load sections.");
                }

                const data = await response.json();
                resetSectionSelect(data.length ? "Select section" : "No sections found");
                data.forEach(item => {
                    sectionSelect.append(buildOption(item.value, item.text, false));
                });
            } catch (error) {
                resetSectionSelect("Failed to load sections");
            } finally {
                sectionSelect.disabled = false;
            }
        };

        warehouseSelect.addEventListener("change", (event) => {
            const target = event.target;
            if (!(target instanceof HTMLSelectElement)) {
                return;
            }
            sectorSelect.value = "";
            resetSectionSelect("Select section");
            loadSectors(target.value);
        });

        sectorSelect.addEventListener("change", (event) => {
            const target = event.target;
            if (!(target instanceof HTMLSelectElement)) {
                return;
            }
            sectionSelect.value = "";
            loadSections(target.value);
        });

        if (warehouseSelect.value && sectorSelect.options.length <= 1) {
            loadSectors(warehouseSelect.value);
        }

        if (sectorSelect.value && sectionSelect.options.length <= 1) {
            loadSections(sectorSelect.value);
        }
    };

    const attachStructureFilterCascade = () => {
        const warehouseSelect = document.querySelector(".js-structure-warehouse-filter");
        const sectorSelect = document.querySelector(".js-structure-sector-filter");
        const sectionSelect = document.querySelector(".js-structure-section-filter");

        if (!(warehouseSelect instanceof HTMLSelectElement)
            || !(sectorSelect instanceof HTMLSelectElement)
            || !(sectionSelect instanceof HTMLSelectElement)) {
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

        const resetSelect = (select, placeholder) => {
            select.innerHTML = "";
            select.append(buildOption("", placeholder, true));
        };

        const loadSectors = async (warehouseId) => {
            resetSelect(sectorSelect, "Loading...");
            resetSelect(sectionSelect, "All sections");
            sectorSelect.disabled = true;

            if (!warehouseId) {
                resetSelect(sectorSelect, "All sectors");
                sectorSelect.disabled = false;
                return;
            }

            try {
                const response = await fetch(`/Structures/SectorOptions?warehouseId=${encodeURIComponent(warehouseId)}`);
                if (!response.ok) {
                    throw new Error("Failed to load sectors.");
                }

                const data = await response.json();
                resetSelect(sectorSelect, data.length ? "All sectors" : "No sectors found");
                data.forEach(item => {
                    sectorSelect.append(buildOption(item.value, item.text, false));
                });
            } catch (error) {
                resetSelect(sectorSelect, "Failed to load sectors");
            } finally {
                sectorSelect.disabled = false;
            }
        };

        const loadSections = async (sectorId) => {
            resetSelect(sectionSelect, "Loading...");
            sectionSelect.disabled = true;

            if (!sectorId) {
                resetSelect(sectionSelect, "All sections");
                sectionSelect.disabled = false;
                return;
            }

            try {
                const response = await fetch(`/Structures/SectionOptions?sectorId=${encodeURIComponent(sectorId)}`);
                if (!response.ok) {
                    throw new Error("Failed to load sections.");
                }

                const data = await response.json();
                resetSelect(sectionSelect, data.length ? "All sections" : "No sections found");
                data.forEach(item => {
                    sectionSelect.append(buildOption(item.value, item.text, false));
                });
            } catch (error) {
                resetSelect(sectionSelect, "Failed to load sections");
            } finally {
                sectionSelect.disabled = false;
            }
        };

        warehouseSelect.addEventListener("change", (event) => {
            const target = event.target;
            if (!(target instanceof HTMLSelectElement)) {
                return;
            }
            sectorSelect.value = "";
            sectionSelect.value = "";
            loadSectors(target.value);
        });

        sectorSelect.addEventListener("change", (event) => {
            const target = event.target;
            if (!(target instanceof HTMLSelectElement)) {
                return;
            }
            sectionSelect.value = "";
            loadSections(target.value);
        });
    };

    const attachLocationWarehouseCascade = () => {
        const warehouseSelect = document.querySelector("[data-location-warehouse]");
        const sectorSelect = document.querySelector("[data-location-sector]");
        const sectionSelect = document.querySelector("[data-location-section]");
        const structureSelect = document.querySelector("[data-location-structure]");
        const zoneSelect = document.querySelector("[data-location-zone]");

        if (!(warehouseSelect instanceof HTMLSelectElement)
            || !(sectorSelect instanceof HTMLSelectElement)
            || !(sectionSelect instanceof HTMLSelectElement)
            || !(structureSelect instanceof HTMLSelectElement)
            || !(zoneSelect instanceof HTMLSelectElement)) {
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

        const resetSelect = (select, placeholder) => {
            select.innerHTML = "";
            select.append(buildOption("", placeholder, true));
        };

        const loadSectors = async (warehouseId) => {
            resetSelect(sectorSelect, "Loading...");
            resetSelect(sectionSelect, "Select section");
            resetSelect(structureSelect, "Select structure");
            resetSelect(zoneSelect, "Select...");
            sectorSelect.disabled = true;

            if (!warehouseId) {
                resetSelect(sectorSelect, "Select sector");
                sectorSelect.disabled = false;
                return;
            }

            try {
                const response = await fetch(`/Locations/SectorOptions?warehouseId=${encodeURIComponent(warehouseId)}`);
                if (!response.ok) {
                    throw new Error("Failed to load sectors.");
                }
                const data = await response.json();
                resetSelect(sectorSelect, data.length ? "Select sector" : "No sectors found");
                data.forEach(item => {
                    sectorSelect.append(buildOption(item.value, item.text, false));
                });
            } catch (error) {
                resetSelect(sectorSelect, "Failed to load sectors");
            } finally {
                sectorSelect.disabled = false;
            }
        };

        const loadSections = async (sectorId) => {
            resetSelect(sectionSelect, "Loading...");
            resetSelect(structureSelect, "Select structure");
            sectionSelect.disabled = true;

            if (!sectorId) {
                resetSelect(sectionSelect, "Select section");
                sectionSelect.disabled = false;
                return;
            }

            try {
                const response = await fetch(`/Locations/SectionOptions?sectorId=${encodeURIComponent(sectorId)}`);
                if (!response.ok) {
                    throw new Error("Failed to load sections.");
                }
                const data = await response.json();
                resetSelect(sectionSelect, data.length ? "Select section" : "No sections found");
                data.forEach(item => {
                    sectionSelect.append(buildOption(item.value, item.text, false));
                });
            } catch (error) {
                resetSelect(sectionSelect, "Failed to load sections");
            } finally {
                sectionSelect.disabled = false;
            }
        };

        const loadStructures = async (sectionId) => {
            resetSelect(structureSelect, "Loading...");
            structureSelect.disabled = true;

            if (!sectionId) {
                resetSelect(structureSelect, "Select structure");
                structureSelect.disabled = false;
                return;
            }

            try {
                const response = await fetch(`/Locations/StructureOptions?sectionId=${encodeURIComponent(sectionId)}`);
                if (!response.ok) {
                    throw new Error("Failed to load structures.");
                }
                const data = await response.json();
                resetSelect(structureSelect, data.length ? "Select structure" : "No structures found");
                data.forEach(item => {
                    structureSelect.append(buildOption(item.value, item.text, false));
                });
            } catch (error) {
                resetSelect(structureSelect, "Failed to load structures");
            } finally {
                structureSelect.disabled = false;
            }
        };

        const loadZones = async (warehouseId) => {
            resetSelect(zoneSelect, "Loading...");
            zoneSelect.disabled = true;

            if (!warehouseId) {
                resetSelect(zoneSelect, "Select...");
                zoneSelect.disabled = false;
                return;
            }

            try {
                const response = await fetch(`/Locations/ZoneOptions?warehouseId=${encodeURIComponent(warehouseId)}`);
                if (!response.ok) {
                    throw new Error("Failed to load zones.");
                }
                const data = await response.json();
                resetSelect(zoneSelect, data.length ? "Select..." : "No zones found");
                data.forEach(item => {
                    zoneSelect.append(buildOption(item.value, item.text, false));
                });
            } catch (error) {
                resetSelect(zoneSelect, "Failed to load zones");
            } finally {
                zoneSelect.disabled = false;
            }
        };

        warehouseSelect.addEventListener("change", (event) => {
            const target = event.target;
            if (!(target instanceof HTMLSelectElement)) {
                return;
            }
            sectorSelect.value = "";
            sectionSelect.value = "";
            structureSelect.value = "";
            zoneSelect.value = "";
            loadSectors(target.value);
            loadZones(target.value);
        });

        sectorSelect.addEventListener("change", (event) => {
            const target = event.target;
            if (!(target instanceof HTMLSelectElement)) {
                return;
            }
            sectionSelect.value = "";
            structureSelect.value = "";
            loadSections(target.value);
        });

        sectionSelect.addEventListener("change", (event) => {
            const target = event.target;
            if (!(target instanceof HTMLSelectElement)) {
                return;
            }
            structureSelect.value = "";
            loadStructures(target.value);
        });

        if (warehouseSelect.value && sectorSelect.options.length <= 1) {
            loadSectors(warehouseSelect.value);
            loadZones(warehouseSelect.value);
        }

        if (sectorSelect.value && sectionSelect.options.length <= 1) {
            loadSections(sectorSelect.value);
        }

        if (sectionSelect.value && structureSelect.options.length <= 1) {
            loadStructures(sectionSelect.value);
        }
    };

    const attachLocationFilterCascade = () => {
        const warehouseSelect = document.querySelector(".js-location-warehouse-filter");
        const sectorSelect = document.querySelector(".js-location-sector-filter");
        const sectionSelect = document.querySelector(".js-location-section-filter");
        const structureSelect = document.querySelector(".js-location-structure-filter");
        const zoneSelect = document.querySelector(".js-location-zone-filter");

        if (!(warehouseSelect instanceof HTMLSelectElement)
            || !(sectorSelect instanceof HTMLSelectElement)
            || !(sectionSelect instanceof HTMLSelectElement)
            || !(structureSelect instanceof HTMLSelectElement)
            || !(zoneSelect instanceof HTMLSelectElement)) {
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

        const resetSelect = (select, placeholder) => {
            select.innerHTML = "";
            select.append(buildOption("", placeholder, true));
        };

        const loadSectors = async (warehouseId) => {
            resetSelect(sectorSelect, "Loading...");
            resetSelect(sectionSelect, "All sections");
            resetSelect(structureSelect, "All structures");
            sectorSelect.disabled = true;

            if (!warehouseId) {
                resetSelect(sectorSelect, "All sectors");
                sectorSelect.disabled = false;
                return;
            }

            try {
                const response = await fetch(`/Locations/SectorOptions?warehouseId=${encodeURIComponent(warehouseId)}`);
                if (!response.ok) {
                    throw new Error("Failed to load sectors.");
                }
                const data = await response.json();
                resetSelect(sectorSelect, data.length ? "All sectors" : "No sectors found");
                data.forEach(item => {
                    sectorSelect.append(buildOption(item.value, item.text, false));
                });
            } catch (error) {
                resetSelect(sectorSelect, "Failed to load sectors");
            } finally {
                sectorSelect.disabled = false;
            }
        };

        const loadSections = async (sectorId) => {
            resetSelect(sectionSelect, "Loading...");
            resetSelect(structureSelect, "All structures");
            sectionSelect.disabled = true;

            if (!sectorId) {
                resetSelect(sectionSelect, "All sections");
                sectionSelect.disabled = false;
                return;
            }

            try {
                const response = await fetch(`/Locations/SectionOptions?sectorId=${encodeURIComponent(sectorId)}`);
                if (!response.ok) {
                    throw new Error("Failed to load sections.");
                }
                const data = await response.json();
                resetSelect(sectionSelect, data.length ? "All sections" : "No sections found");
                data.forEach(item => {
                    sectionSelect.append(buildOption(item.value, item.text, false));
                });
            } catch (error) {
                resetSelect(sectionSelect, "Failed to load sections");
            } finally {
                sectionSelect.disabled = false;
            }
        };

        const loadStructures = async (sectionId) => {
            resetSelect(structureSelect, "Loading...");
            structureSelect.disabled = true;

            if (!sectionId) {
                resetSelect(structureSelect, "All structures");
                structureSelect.disabled = false;
                return;
            }

            try {
                const response = await fetch(`/Locations/StructureOptions?sectionId=${encodeURIComponent(sectionId)}`);
                if (!response.ok) {
                    throw new Error("Failed to load structures.");
                }
                const data = await response.json();
                resetSelect(structureSelect, data.length ? "All structures" : "No structures found");
                data.forEach(item => {
                    structureSelect.append(buildOption(item.value, item.text, false));
                });
            } catch (error) {
                resetSelect(structureSelect, "Failed to load structures");
            } finally {
                structureSelect.disabled = false;
            }
        };

        const loadZones = async (warehouseId) => {
            resetSelect(zoneSelect, "Loading...");
            zoneSelect.disabled = true;

            if (!warehouseId) {
                resetSelect(zoneSelect, "All zones");
                zoneSelect.disabled = false;
                return;
            }

            try {
                const response = await fetch(`/Locations/ZoneOptions?warehouseId=${encodeURIComponent(warehouseId)}`);
                if (!response.ok) {
                    throw new Error("Failed to load zones.");
                }
                const data = await response.json();
                resetSelect(zoneSelect, data.length ? "All zones" : "No zones found");
                data.forEach(item => {
                    zoneSelect.append(buildOption(item.value, item.text, false));
                });
            } catch (error) {
                resetSelect(zoneSelect, "Failed to load zones");
            } finally {
                zoneSelect.disabled = false;
            }
        };

        warehouseSelect.addEventListener("change", (event) => {
            const target = event.target;
            if (!(target instanceof HTMLSelectElement)) {
                return;
            }
            sectorSelect.value = "";
            sectionSelect.value = "";
            structureSelect.value = "";
            zoneSelect.value = "";
            loadSectors(target.value);
            loadZones(target.value);
        });

        sectorSelect.addEventListener("change", (event) => {
            const target = event.target;
            if (!(target instanceof HTMLSelectElement)) {
                return;
            }
            sectionSelect.value = "";
            structureSelect.value = "";
            loadSections(target.value);
        });

        sectionSelect.addEventListener("change", (event) => {
            const target = event.target;
            if (!(target instanceof HTMLSelectElement)) {
                return;
            }
            structureSelect.value = "";
            loadStructures(target.value);
        });
    };

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", () => {
            attachIndexHeaderActions();
            attachSectionWarehouseCascade();
            attachSectionFilterCascade();
            attachStructureWarehouseCascade();
            attachStructureFilterCascade();
            attachLocationWarehouseCascade();
            attachLocationFilterCascade();
        });
    } else {
        attachIndexHeaderActions();
        attachSectionWarehouseCascade();
        attachSectionFilterCascade();
        attachStructureWarehouseCascade();
        attachStructureFilterCascade();
        attachLocationWarehouseCascade();
        attachLocationFilterCascade();
    }
})();
