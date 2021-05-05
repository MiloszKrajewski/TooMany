import { ReactNode } from 'react';

export default function ({
	children,
	className,
}: {
	children: ReactNode;
	className?: string;
}) {
	return (
		<div
			className={`bg-gray-200 text-gray-900 font-bold sticky top-0 ${className}`}
		>
			{children}
		</div>
	);
}
