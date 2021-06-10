import { useMediaQuery } from 'react-responsive';

export default function () {
	const isPortrait = useMediaQuery({ orientation: 'portrait' });
	const isWide = useMediaQuery({ minWidth: 960 });
	return {
		isPortrait,
		isWide,
	};
}
