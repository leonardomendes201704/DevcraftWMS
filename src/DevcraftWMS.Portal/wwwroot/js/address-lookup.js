(() => {
    const components = document.querySelectorAll('[data-address-component="true"]');
    if (!components.length) {
        return;
    }

    const normalize = (value) => (value || '').toString().trim();
    const digitsOnly = (value) => normalize(value).replace(/\D/g, '');

    const fetchJson = async (url) => {
        const response = await fetch(url, { headers: { 'Accept': 'application/json' } });
        if (!response.ok) {
            throw new Error(`Request failed with status ${response.status}`);
        }
        return response.json();
    };

    const setSelectOptions = (select, items, selected) => {
        select.innerHTML = '';
        const placeholder = document.createElement('option');
        placeholder.value = '';
        placeholder.textContent = 'Select';
        select.appendChild(placeholder);

        items.forEach((item) => {
            const option = document.createElement('option');
            option.value = item.value;
            option.textContent = item.label;
            select.appendChild(option);
        });

        if (selected) {
            select.value = selected;
        }
    };

    const initComponent = (component) => {
        const cepEnabled = component.dataset.cepEnabled === 'true';
        const ibgeEnabled = component.dataset.ibgeEnabled === 'true';
        const statesUrl = component.dataset.statesUrl;
        const citiesUrl = component.dataset.citiesUrl;
        const cepUrl = component.dataset.cepUrl;

        const postalCodeInput = component.querySelector('[data-address-field="postalCode"]');
        const addressLine1Input = component.querySelector('[data-address-field="addressLine1"]');
        const districtInput = component.querySelector('[data-address-field="district"]');
        const citySelect = component.querySelector('[data-address-field="city"]');
        const stateSelect = component.querySelector('[data-address-field="state"]');
        const countryInput = component.querySelector('[data-address-field="country"]');
        const lookupButton = component.querySelector('[data-address-lookup="cep"]');

        const loadStates = async () => {
            if (!ibgeEnabled || !stateSelect || !statesUrl) {
                return;
            }
            const data = await fetchJson(statesUrl);
            const selected = normalize(stateSelect.dataset.currentValue || stateSelect.value);
            setSelectOptions(
                stateSelect,
                data.map((item) => ({ value: item.code, label: item.name })),
                selected);
        };

        const loadCities = async (stateCode) => {
            if (!ibgeEnabled || !citySelect || !citiesUrl || !stateCode) {
                return;
            }

            const data = await fetchJson(`${citiesUrl}?uf=${encodeURIComponent(stateCode)}`);
            const selected = normalize(citySelect.dataset.currentValue || citySelect.value);
            setSelectOptions(
                citySelect,
                data.map((item) => ({ value: item.name, label: item.name })),
                selected);
        };

        const applyCepLookup = async () => {
            if (!cepEnabled || !postalCodeInput || !cepUrl) {
                return;
            }

            const cep = digitsOnly(postalCodeInput.value);
            if (!cep || cep.length < 8) {
                return;
            }

            const data = await fetchJson(`${cepUrl}?cep=${encodeURIComponent(cep)}`);
            if (addressLine1Input && data.addressLine1) {
                addressLine1Input.value = data.addressLine1;
            }
            if (districtInput && data.district) {
                districtInput.value = data.district;
            }
            if (stateSelect && data.state) {
                stateSelect.value = data.state;
                await loadCities(data.state);
            }
            if (citySelect && data.city) {
                citySelect.value = data.city;
            }
            if (countryInput && data.country) {
                countryInput.value = data.country;
            }
            if (postalCodeInput && data.postalCode) {
                postalCodeInput.value = data.postalCode;
            }
        };

        if (lookupButton) {
            lookupButton.addEventListener('click', (event) => {
                event.preventDefault();
                applyCepLookup().catch(() => {});
            });
        }

        if (postalCodeInput) {
            postalCodeInput.addEventListener('blur', () => {
                if (cepEnabled) {
                    applyCepLookup().catch(() => {});
                }
            });
        }

        if (stateSelect) {
            stateSelect.addEventListener('change', () => {
                loadCities(stateSelect.value).catch(() => {});
            });
        }

        loadStates()
            .then(() => {
                const currentState = normalize(stateSelect?.value);
                if (currentState) {
                    return loadCities(currentState);
                }
                return Promise.resolve();
            })
            .catch(() => {});
    };

    components.forEach(initComponent);
})();
