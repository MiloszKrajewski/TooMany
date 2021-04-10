import { ReactNode } from 'react';

export default ({ children }: { children: ReactNode }) => {
	return <li className="font-bold">{children}</li>;
};
