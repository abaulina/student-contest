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
			const userInfo = await response.json();
			return userInfo;
		} else throw new Error(response);
	} catch (error) {
		if (!error.status) error.status = 500;
		handleError(error.status);
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

export async function sendLoginRequest(loginCredentials) {
	try {
		const response = await fetch(`${url}/login`, {
			method: 'POST',
			credentials: 'include',
			headers: {
				'Content-Type': 'application/json'
			},
			body: JSON.stringify(loginCredentials)
		});
		if (response.ok) {
			const data = await response.json();
			return data.token;
		} else throw new Error(response);
	} catch (error) {
		if (!error.status) error.status = 500;
		handleError(error.status);
	}
}

export async function sendRefreshRequest() {
	try {
		const response = await fetch(`${url}/refresh-token`, {
			credentials: 'include',
			method: 'POST',
			headers: {
				'Content-Type': 'application/json'
			}
		});
		if (response.ok) {
			const data = await response.json();
			return data.token;
		} else throw new Error(response);
	} catch (error) {
		if (!error.status) error.status = 500;
		handleError(error.status);
	}
}

export async function sendLogoutRequest() {
	const response = await fetch(`${url}/logout`, {
		credentials: 'include',
		method: 'DELETE'
	});
	return handleResponse(response);
}

function handleResponse(response) {
	try {
		if (response.ok) {
			return true;
		} else throw new Error(response);
	} catch (error) {
		if (!error.status) error.status = 500;
		handleError(error.status);
	}
}
