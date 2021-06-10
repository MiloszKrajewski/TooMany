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

async function fetcher<Result>(
	url: string,
	method: string,
	headers?: { [EHeaders.contentType]: string },
	body?: string,
): Promise<Result> {
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
		postRequest: <Result, Payload>(
			url: string,
			payload: Payload,
		): Promise<Result> => {
			return fetcher<Result>(
				url,
				'POST',
				headers,
				JSON.stringify(payload),
			);
		},
		getRequest: <Result,>(url: string): Promise<Result> => {
			return fetcher<Result>(url, 'GET');
		},
		putRequest: <Result, Payload>(
			url: string,
			payload?: Payload,
		): Promise<Result> => {
			if (payload) {
				return fetcher<Result>(
					url,
					'PUT',
					headers,
					JSON.stringify(payload),
				);
			}
			return fetcher<Result>(url, 'PUT');
		},
		deleteRequest: <Result,>(url: string): Promise<Result> => {
			return fetcher<Result>(url, 'DELETE');
		},
	};
}
