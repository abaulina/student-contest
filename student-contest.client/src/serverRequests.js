import handleError from './errors/errorHandler';
import configData from './config.json';

const url = configData.SERVER_URL;

export async function getUserInfo(accessToken) {
	try {
		const response = await fetch(url, {
			method: 'GET',
			headers: {
				Authorization: `Bearer ${accessToken}`
			}
		});
		if (response.ok) {
			const data = await response.json();
			return { userInfo: data };
		} else handleError(response);
	} catch (error) {
		console.error(error);
	}
}

export async function registerUser(newUser) {
	const response = await fetch(`${url}/register`, {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json'
		},
		body: JSON.stringify(newUser)
	});
	return handleResponse(response);
}

export async function login(loginCredentials) {
	try {
		const response = await fetch(`${url}/login`, {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json'
			},
			body: JSON.stringify(loginCredentials)
		});
		if (response.ok) {
			const data = await response.json();
			return { accessToken: data };
		} else handleError(response);
	} catch (error) {
		console.error(error);
	}
}

export async function refreshToken() {
	try {
		const response = await fetch(`${url}/refresh-token`, {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json'
			}
		});
		if (response.ok) {
			const data = await response.json();
			return { accessToken: data };
		} else handleError(response);
	} catch (error) {
		console.error(error);
	}
}

export async function logout() {
	const response = await fetch(`${url}/logout`, {
		method: 'DELETE'
	});
	return handleResponse(response);
}

function handleResponse(response) {
	try {
		if (response.ok) {
			return true;
		} else handleError(response);
	} catch (error) {
		console.error(error);
	}
}
