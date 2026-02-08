(() => {
    const forms = document.querySelectorAll('[data-warehouse-capacity="true"]');
    if (!forms.length) {
        return;
    }

    const formatPhone = (value) => {
        const digits = (value || '').replace(/\D/g, '').slice(0, 11);
        if (!digits) {
            return '';
        }
        if (digits.length <= 10) {
            return digits.replace(/(\d{2})(\d{0,4})(\d{0,4})/, (_, d1, d2, d3) => {
                return `(${d1})${d2 ? ' ' + d2 : ''}${d3 ? '-' + d3 : ''}`;
            }).trim();
        }
        return digits.replace(/(\d{2})(\d{0,5})(\d{0,4})/, (_, d1, d2, d3) => {
            return `(${d1})${d2 ? ' ' + d2 : ''}${d3 ? '-' + d3 : ''}`;
        }).trim();
    };

    const setupPhoneMask = () => {
        const phoneInputs = document.querySelectorAll('[data-mask="br-phone"]');
        phoneInputs.forEach((input) => {
            input.addEventListener('input', () => {
                input.value = formatPhone(input.value);
                const end = input.value.length;
                input.setSelectionRange(end, end);
            });
        });
    };

    const setupCostCenter = () => {
        const select = document.querySelector('[data-cost-center-select="true"]');
        const nameInput = document.querySelector('[data-cost-center-name="true"]');
        if (!select || !nameInput) {
            return;
        }

        const applyName = () => {
            const selected = select.options[select.selectedIndex];
            nameInput.value = selected?.dataset.costCenterName || '';
        };

        select.addEventListener('change', applyName);
        applyName();
    };

    const setupCapacity = () => {
        const lengthInput = document.querySelector('[data-capacity-length="true"]');
        const widthInput = document.querySelector('[data-capacity-width="true"]');
        const areaInput = document.querySelector('[data-capacity-area="true"]');
        if (!lengthInput || !widthInput || !areaInput) {
            return;
        }

        const calculateArea = () => {
            const length = parseFloat(lengthInput.value.replace(',', '.'));
            const width = parseFloat(widthInput.value.replace(',', '.'));
            if (!Number.isFinite(length) || !Number.isFinite(width)) {
                areaInput.value = '';
                return;
            }
            const area = length * width;
            areaInput.value = area ? area.toFixed(2) : '';
        };

        lengthInput.addEventListener('input', calculateArea);
        widthInput.addEventListener('input', calculateArea);
        calculateArea();
    };

    setupPhoneMask();
    setupCostCenter();
    setupCapacity();
})();
