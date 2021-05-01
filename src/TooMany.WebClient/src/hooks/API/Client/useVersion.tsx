const fakeRqResponse = {
	data: env.version || '0.0.0',
	isLoading: false,
	isFetching: false,
	isError: false,
	isSuccess: true,
};

export default function () {
	return fakeRqResponse;
}
