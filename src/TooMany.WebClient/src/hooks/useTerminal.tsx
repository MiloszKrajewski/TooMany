import { useEffect, useRef } from 'react';
import { Terminal } from 'xterm';
import { FitAddon } from 'xterm-addon-fit';
import { WebglAddon } from 'xterm-addon-webgl';

export default function useTerminal(
	id: string,
	container: HTMLElement | null,
): Terminal {
	const instance = useRef<Terminal>(new Terminal({ disableStdin: true }));
	const fitAddon = useRef<FitAddon>(new FitAddon());

	useEffect(() => {
		if (container === null) {
			return;
		}

		const xterm = instance.current;

		xterm.open(container);

		xterm.loadAddon(fitAddon.current);
		const webglAddon = new WebglAddon();
		xterm.loadAddon(webglAddon);

		const resizeEvent = () => fitAddon.current.fit();
		resizeEvent();
		window.addEventListener('resize', resizeEvent);
		return function () {
			window.removeEventListener('resize', resizeEvent);
			xterm.reset();
		};
	}, [container]);

	useEffect(() => {
		fitAddon.current.fit();
	}, [id]);

	return instance.current;
}
