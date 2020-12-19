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

async function fetcher<T>(url: string, method: string): Promise<T> {
	const res = await fetch(url, { method });
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

export default function () {
	return {
		getRequest: <T>(url: string): Promise<T> => fetcher<T>(url, 'GET'),
		putRequest: <T>(url: string): Promise<T> => fetcher<T>(url, 'PUT'),
		deleteRequest: <T>(url: string): Promise<T> => fetcher<T>(url, 'DELETE'),
	};
}
