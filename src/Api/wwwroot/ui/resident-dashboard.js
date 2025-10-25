const state = {
    requestId: null,
    phone: "+910000000001",
    accessToken: null,
    refreshToken: null,
    flats: [],
    helpers: []
};

const authStatus = document.getElementById("auth-status");
const searchStatus = document.getElementById("search-status");
const bookingStatus = document.getElementById("booking-status");
const bookingJson = document.getElementById("booking-json");
const helpersTable = document.getElementById("helpers-table");
const helpersTableBody = helpersTable.querySelector("tbody");
const flatSection = document.getElementById("flat-section");
const searchSection = document.getElementById("search-section");
const bookingSection = document.getElementById("booking-section");
const otpRequestForm = document.getElementById("otp-request-form");
const otpVerifyForm = document.getElementById("otp-verify-form");
const flatSelect = document.getElementById("flat-select");
const searchForm = document.getElementById("search-form");
const startTimeInput = document.getElementById("start-time");
const endTimeInput = document.getElementById("end-time");
const phoneInput = document.getElementById("phone");

startTimeInput.value = defaultDateTimeLocal(1, 9);
endTimeInput.value = defaultDateTimeLocal(1, 11);

otpRequestForm.addEventListener("submit", async (event) => {
    event.preventDefault();
    const phone = phoneInput.value.trim();
    if (!phone) {
        renderStatus(authStatus, "Phone number is required", true);
        return;
    }

    try {
        const response = await fetchJson("/auth/otp/request", {
            method: "POST",
            body: JSON.stringify({ phone })
        });
        state.requestId = response.requestId;
        state.phone = phone;
        otpVerifyForm.hidden = false;
        renderStatus(authStatus, `OTP sent. Use 123456 for the seeded fake provider.`);
    } catch (error) {
        renderStatus(authStatus, error.message, true);
    }
});

otpVerifyForm.addEventListener("submit", async (event) => {
    event.preventDefault();
    const code = document.getElementById("otp-code").value.trim();
    if (!code) {
        renderStatus(authStatus, "Enter the OTP code", true);
        return;
    }

    try {
        const response = await fetchJson("/auth/otp/verify", {
            method: "POST",
            body: JSON.stringify({ requestId: state.requestId, code, phone: state.phone, role: "Resident" })
        });
        state.accessToken = response.accessToken;
        state.refreshToken = response.refreshToken;
        renderStatus(authStatus, `Authenticated as ${response.user.name ?? response.user.phone}. Tokens cached for this session.`);
        otpRequestForm.querySelector("button").disabled = true;
        otpVerifyForm.querySelector("button").disabled = true;
        await loadFlats();
        flatSection.hidden = false;
        searchSection.hidden = false;
        bookingSection.hidden = false;
    } catch (error) {
        renderStatus(authStatus, error.message, true);
    }
});

flatSelect.addEventListener("change", () => {
    if (!flatSelect.value) {
        renderStatus(searchStatus, "Select a flat to proceed", true);
    } else {
        renderStatus(searchStatus, "", false);
    }
});

searchForm.addEventListener("submit", async (event) => {
    event.preventDefault();
    if (!state.accessToken) {
        renderStatus(searchStatus, "Authenticate first", true);
        return;
    }
    if (!flatSelect.value) {
        renderStatus(searchStatus, "Select a flat before searching", true);
        return;
    }

    const formData = new FormData(searchForm);
    const params = new URLSearchParams();
    for (const [key, value] of formData.entries()) {
        if (value) {
            if (key === "start" || key === "end") {
                params.append(key === "start" ? "startAt" : "endAt", new Date(value).toISOString());
            } else {
                params.append(key, value.toString());
            }
        }
    }

    try {
        const response = await authorizedFetch(`/helpers/search?${params.toString()}`);
        const body = await response.json();
        state.helpers = body.helpers ?? [];
        if (state.helpers.length === 0) {
            helpersTable.hidden = true;
            renderStatus(searchStatus, "No helpers matched the criteria.");
            return;
        }
        renderHelpers(state.helpers);
        renderStatus(searchStatus, `Found ${state.helpers.length} helper(s). Select one to book.`);
    } catch (error) {
        renderStatus(searchStatus, error.message, true);
    }
});

helpersTableBody.addEventListener("click", async (event) => {
    const target = event.target;
    if (!(target instanceof HTMLButtonElement)) {
        return;
    }

    const helperId = target.dataset.helperId;
    if (!helperId) {
        return;
    }

    try {
        target.disabled = true;
        const startAt = new Date(startTimeInput.value).toISOString();
        const endAt = new Date(endTimeInput.value).toISOString();
        const body = {
            helperId,
            flatId: flatSelect.value,
            serviceType: document.getElementById("skill").value || "General",
            startAt,
            endAt,
            paymentMethod: "Razorpay",
            notes: "Booked via resident dashboard demo"
        };
        const response = await authorizedFetch("/bookings", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(body)
        });

        if (!response.ok) {
            const errorBody = await response.text();
            throw new Error(`Booking failed: ${response.status} ${errorBody}`);
        }

        const booking = await response.json();
        renderStatus(bookingStatus, `Booking created with state ${booking.state}.`);
        bookingJson.textContent = JSON.stringify(booking, null, 2);
        bookingSection.scrollIntoView({ behavior: "smooth" });
    } catch (error) {
        renderStatus(bookingStatus, error.message, true);
    } finally {
        target.disabled = false;
    }
});

function renderHelpers(helpers) {
    helpersTableBody.innerHTML = "";
    for (const helper of helpers) {
        const row = document.createElement("tr");
        row.innerHTML = `
            <td>${helper.name ?? "Unknown"}</td>
            <td>${helper.skills.join(", ")}</td>
            <td>${helper.baseRatePerHour}</td>
            <td>${helper.ratingAvg.toFixed(1)}</td>
            <td><button data-helper-id="${helper.helperId}">Book</button></td>
        `;
        helpersTableBody.appendChild(row);
    }
    helpersTable.hidden = false;
}

async function loadFlats() {
    const response = await authorizedFetch("/residents/flats");
    const body = await response.json();
    state.flats = body.flats ?? [];
    flatSelect.innerHTML = "";
    for (const flat of state.flats) {
        const option = document.createElement("option");
        option.value = flat.flatId;
        option.textContent = `${flat.number} (${flat.buildingName})`;
        flatSelect.appendChild(option);
    }
    if (state.flats.length === 0) {
        renderStatus(searchStatus, "No flats assigned. Contact support.", true);
    }
}

async function authorizedFetch(url, options = {}, retry = true) {
    const headers = new Headers(options.headers || {});
    headers.set("Accept", "application/json");
    if (options.body && !headers.has("Content-Type")) {
        headers.set("Content-Type", "application/json");
    }
    if (state.accessToken) {
        headers.set("Authorization", `Bearer ${state.accessToken}`);
    }
    const response = await fetch(url, { ...options, headers });
    if (response.status === 401 && retry && state.refreshToken) {
        const refreshed = await refreshTokens();
        if (refreshed) {
            return authorizedFetch(url, options, false);
        }
    }
    if (!response.ok) {
        throw new Error(`Request failed (${response.status})`);
    }
    return response;
}

async function refreshTokens() {
    try {
        const response = await fetchJson("/auth/refresh", {
            method: "POST",
            body: JSON.stringify({ refreshToken: state.refreshToken })
        });
        state.accessToken = response.accessToken;
        state.refreshToken = response.refreshToken;
        return true;
    } catch (error) {
        renderStatus(authStatus, `Refresh failed: ${error.message}`, true);
        return false;
    }
}

async function fetchJson(url, options) {
    const headers = new Headers(options?.headers || {});
    headers.set("Content-Type", "application/json");
    headers.set("Accept", "application/json");
    const response = await fetch(url, { ...options, headers });
    if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`Request failed (${response.status}): ${errorText}`);
    }
    return await response.json();
}

function renderStatus(target, message, isError = false) {
    target.textContent = message;
    target.classList.toggle("error", Boolean(isError));
}

function defaultDateTimeLocal(dayOffset, hour) {
    const date = new Date();
    date.setDate(date.getDate() + dayOffset);
    date.setHours(hour, 0, 0, 0);
    const offset = date.getTimezoneOffset();
    date.setMinutes(date.getMinutes() - offset);
    return date.toISOString().slice(0, 16);
}
