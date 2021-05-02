import type { ReactNode } from 'react';
import { Suspense, memo } from 'react';
import { QueryErrorResetBoundary } from 'react-query';
import { ErrorBoundary } from 'react-error-boundary';

const SuspenseQuery = function ({
	children,
	fallback = null,
}: {
	children?: ReactNode;
	fallback?: ReactNode;
}) {
	return (
		<QueryErrorResetBoundary>
			{({ reset }) => (
				<ErrorBoundary
					fallbackRender={({ error, resetErrorBoundary }) => (
						<div>
							There was an error!{' '}
							<button onClick={() => resetErrorBoundary()}>Try again</button>
							<pre style={{ whiteSpace: 'normal' }}>{error.message}</pre>
						</div>
					)}
					onReset={reset}
				>
					<Suspense fallback={fallback}>{children}</Suspense>
				</ErrorBoundary>
			)}
		</QueryErrorResetBoundary>
	);
};

export default memo(SuspenseQuery);
