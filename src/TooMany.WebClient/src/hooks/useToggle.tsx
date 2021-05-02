import { useState } from 'react';

export default function (initial: boolean) {
	const [isToggled, setIsToggled] = useState<boolean>(initial);
	return [isToggled, () => setIsToggled((x) => !x)];
}
