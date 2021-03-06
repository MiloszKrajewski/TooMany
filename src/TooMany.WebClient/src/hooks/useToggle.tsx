import { useState } from 'react';

export default function (initial = false): [boolean, () => void] {
	const [isToggled, setIsToggled] = useState<boolean>(initial);
	return [isToggled, () => setIsToggled((x) => !x)];
}
