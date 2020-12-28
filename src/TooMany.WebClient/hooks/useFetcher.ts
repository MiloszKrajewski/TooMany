function fail({
	redirected,
	status,
	statusText,
	type,
	url,
	method,
}: {
	redirected: boolean;
	status: number;
	statusText: string;
	type: string;
	url: string;
	method: string;
}) {
	return `
    Request failed:
      - url: ${url}
      - status: ${status}
      - statusText: ${statusText}
      - method: ${method}
      - redirected: ${redirected}
      - type: ${type}
`;
}

enum EHeaders {
	contentType = 'Content-Type',
}

async function fetcher<T>(
	url: string,
	method: string,
	headers?: { [EHeaders.contentType]: string },
	body?: string,
): Promise<T> {
	const res = await fetch(url, { method, headers, body });
	if (res.status < 200 || res.status >= 300) {
		throw new Error(
			fail({
				redirected: res.redirected,
				status: res.status,
				statusText: res.statusText,
				type: res.type,
				url: res.url,
				method,
			}),
		);
	}
	const { result } = await res.json();
	return result;
}

const headers = { [EHeaders.contentType]: 'application/json' };
export default function () {
	return {
		postRequest: <T, D>(url: string, payload: D): Promise<T> =>
			fetcher<T>(url, 'POST', headers, JSON.stringify(payload)),
		getRequest: <T>(url: string): Promise<T> => fetcher<T>(url, 'GET'),
		putRequest: <T>(url: string): Promise<T> => fetcher<T>(url, 'PUT', headers),
		deleteRequest: <T>(url: string): Promise<T> => fetcher<T>(url, 'DELETE'),
	};
}
