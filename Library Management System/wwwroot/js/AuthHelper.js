


/**
 * Send an HTTP request using fetch while adding an X-CSRF-TOKEN header and including credentials.
 *
 * Merges any user-supplied headers with an `X-CSRF-TOKEN` header obtained from getCsrfToken and forces `credentials: 'include'`.
 * @param {string} url - Request URL.
 * @param {object} [options] - Fetch options (e.g., method, body, headers); provided headers will be merged with the CSRF header.
 * @returns {Promise<Response>} The Response returned by fetch.
 */
 async function secureFetch(url, options = {}) {
    const csrfToken = await getCsrfToken();

    const defaultHeaders = {
        'X-CSRF-TOKEN': csrfToken
    };

    const fetchOptions = {
        ...options,
        headers: {
            ...defaultHeaders,
            ...(options.headers || {})
        },
        credentials: 'include' // always include cookies
    };

    return fetch(url, fetchOptions);
}