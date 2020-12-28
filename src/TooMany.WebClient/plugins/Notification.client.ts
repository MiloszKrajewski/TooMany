/* eslint-disable no-new */
import { Plugin } from '@nuxt/types';

class NotificationManager {
	private permission: boolean = false;

	constructor() {
		this.getPermission();
	}

	async getPermission() {
		const permission = await Notification.requestPermission();
		this.permission = permission === 'granted';
	}

	create({
		title,
		body,
		requireInteraction = false,
	}: {
		title: string;
		body: string;
		requireInteraction?: boolean;
	}) {
		if (!this.permission) return;
		new Notification(title, {
			body,
			icon: '/icons/icon-64',
			requireInteraction,
		});
	}
}

declare module 'vue/types/vue' {
	interface Vue {
		$Notification: NotificationManager;
	}
}

declare module '@nuxt/types' {
	interface NuxtAppOptions {
		$Notification: NotificationManager;
	}
	interface Context {
		$Notification: NotificationManager;
	}
}

const $Notification: Plugin = (_, inject) => {
	inject('Notification', new NotificationManager());
};

export default $Notification;
