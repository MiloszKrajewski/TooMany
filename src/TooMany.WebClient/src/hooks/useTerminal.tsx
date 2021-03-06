import { useEffect, useRef } from 'react';
import { Terminal } from 'xterm';
import { FitAddon } from 'xterm-addon-fit';
import { WebglAddon } from 'xterm-addon-webgl';

export default function useTerminal(
	id: string,
	container: HTMLElement | null,
	initialLogs = '',
): Terminal {
	const instance = useRef<Terminal>(new Terminal({ disableStdin: true }));
	const fitAddon = useRef<FitAddon>(new FitAddon());
	const webglAddon = useRef<WebglAddon>(new WebglAddon());

	useEffect(() => {
		if (container === null) {
			return;
		}

		instance.current.open(container);
		instance.current.loadAddon(webglAddon.current);
		instance.current.loadAddon(fitAddon.current);
		fitAddon.current.fit();
	}, [container]);

	useEffect(() => {
		fitAddon.current.fit();
		return function () {
			instance.current.reset();
		};
	}, [id]);

	useEffect(() => {
		const resizeEvent = () => fitAddon.current.fit();
		window.addEventListener('resize', resizeEvent);
		return function () {
			window.removeEventListener('resize', resizeEvent);
		};
	}, [id]);

	useEffect(() => {
		if (typeof instance.current === 'undefined') {
			return;
		}
		instance.current.write(`${initialLogs}\r\n`, () => {
			fitAddon.current.fit();
		});
	}, [id, initialLogs]);

	return instance.current;
}
