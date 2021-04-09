import type { ReactNode } from 'react';

export default ({ children }: { children?: ReactNode }) => (
	<button
		type="button"
		className={`
            focus:outline-none
            text-white
            text-sm
            py-2.5
            px-5
            rounded-md
            bg-blue-500
            hover:bg-blue-600
            hover:shadow-lg
        `}
	>
		{children}
	</button>
);
