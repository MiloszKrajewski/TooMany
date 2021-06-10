import { ReactNode } from 'react';
import { Link } from 'react-router-dom';

export default function ({
	className,
	children,
	to,
}: {
	className?: string;
	children: ReactNode;
	to: string;
}) {
	return (
		<Link className={className} to={to}>
			{children}
		</Link>
	);
}
