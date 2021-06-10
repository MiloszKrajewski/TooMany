import 'xterm/css/xterm.css';
import { useEffect, useRef, useMemo } from 'react';
import { useQueryClient } from 'react-query';

import type * as Task from 'types/task';

import SignalR from '@tm/SignalR';

import { useLog } from '@hooks/API/Task/log';
import { getQueryKey } from '@hooks/API/Task/log/helpers';
import useTerminal from '@hooks/useTerminal';

import { formatLine } from './helpers';

function filterLog(value: string, log: Task.ILog, isRegExp = false) {
	if (isRegExp) {
		return new RegExp(value).test(log.text);
	} else {
		return log.text.indexOf(value) !== -1;
	}
}

export default function ({ name }: { name: string }) {
	const { data: logs = [], isLoading } = useLog(name);
	const queryClient = useQueryClient();

	const isRegExp = useRef(false);
	const isFiltered = useRef(false);
	const input = useRef<HTMLInputElement>(null);
	const container = useRef<HTMLDivElement>(null);

	const id = `task/${name}`;

	const initialLogs = useMemo(() => {
		return logs
			.map((log) => formatLine(log.text, log.timestamp))
			.join('\r\n');
	}, [id, logs]);

	const [xterm, { resizeTerminal }] = useTerminal(
		id,
		container.current,
		initialLogs,
	);

	useEffect(() => {
		if (typeof xterm === 'undefined') {
			return;
		}

		const fn = SignalR.onTaskLog(name, (_, log) => {
			if (!log.text) return;
			if (isFiltered.current) {
				if (
					input.current !== null &&
					filterLog(input.current.value, log, isRegExp.current)
				) {
					xterm.writeln(formatLine(log.text, log.timestamp));
				}
			} else {
				xterm.writeln(formatLine(log.text, log.timestamp));
			}
			queryClient.setQueryData<Task.ILog[]>(
				getQueryKey(name),
				(state) => {
					if (!state) return [log];
					state.push(log);
					return state;
				},
			);
		});
		return () => {
			if (typeof fn === 'function') {
				SignalR.offTaskLog(fn);
			}
		};
	}, [xterm, id]);

	const handleFilter = () => {
		if (input.current === null) {
			isFiltered.current = false;
		} else {
			isFiltered.current = input.current.value.length > 0;
		}
		let parsedLogs = logs;
		if (isFiltered.current) {
			parsedLogs = logs.filter((log) => {
				if (input.current === null) return true;
				return filterLog(input.current.value, log, isRegExp.current);
			});
		}
		const filteredLogs = parsedLogs
			.map((log) => formatLine(log.text, log.timestamp))
			.join('\r\n');
		xterm.reset();
		xterm.write(`${filteredLogs}\r\n`, () => {
			resizeTerminal();
		});
	};

	const toggleRegExp = () => {
		isRegExp.current = !isRegExp.current;
	};

	if (isLoading) return null;
	return (
		<div
			style={{ flex: '1 100%', display: 'flex', flexDirection: 'column' }}
		>
			<input className="text-black" type="text" ref={input} />
			<button onClick={handleFilter}>Trigger filter</button>
			<button onClick={toggleRegExp}>
				Is {isRegExp.current ? 'Not' : ''} Regexp?
			</button>
			<div style={{ flex: '1 100%' }} ref={container} />
		</div>
	);
}
