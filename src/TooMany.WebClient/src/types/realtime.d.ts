import type * as Task from './task';

export type onLogFn = (task: string, data: Task.ILog) => void;
export type onLogFnCallback = (task: string, data: Task.ILog) => void;

export type onMetaFn = (task: string, data: Task.IMeta) => void;
export type onMetaFnCallback = (task: string, data: Task.IMeta) => void;

export type ChannelId = string | null;
