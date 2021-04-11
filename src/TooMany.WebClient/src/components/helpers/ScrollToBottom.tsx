import type { ReactNode } from 'react';
import { memo } from 'react';

const ScrollToBottom = function ({ children }: { children?: ReactNode }) {
	return (
		<div className="flex overflow-y-auto flex-col-reverse">{children}</div>
	);
};

export default memo(ScrollToBottom);
