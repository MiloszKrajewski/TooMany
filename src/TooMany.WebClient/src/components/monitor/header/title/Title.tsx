import type { ReactNode } from 'react';

export default function ({ children }: { children: ReactNode }) {
	return <h3 className="font-bold">{children}</h3>;
}
