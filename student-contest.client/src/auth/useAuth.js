import React, { createContext, useContext, useEffect, useState } from 'react';
import PropTypes from 'prop-types';
import {
	sendRefreshRequest,
	sendLogoutRequest,
	sendLoginRequest
} from '../serverRequests';

const authContext = createContext();

function useProvideAuth() {
	const [accessToken, setAccessToken] = useState('');
	const [isAuthenticated, setAuthenticated] = useState(
		accessToken ? true : false
	);

	useEffect(() => {
		if (isAuthenticated) refreshToken();
	}, []);

	const login = async (loginCredentials) => {
		const accessToken = await sendLoginRequest(loginCredentials);
		if (accessToken) {
			setAccessToken(accessToken);
			setAuthenticated(true);
		}
	};

	const logout = async () => {
		setAccessToken(null);
		setAuthenticated(false);
		await sendLogoutRequest();
	};

	const refreshToken = async () => {
		const accessToken = await sendRefreshRequest();
		if (accessToken) {
			setAccessToken(accessToken);
			setAuthenticated(true);
		} else setAuthenticated(false);
		setTimeout(async () => {
			refreshToken();
		}, 15 * 60000 - 1000);
	};

	return {
		isAuthenticated,
		accessToken,
		login,
		logout
	};
}

export function AuthProvider({ children }) {
	const auth = useProvideAuth();

	return <authContext.Provider value={auth}>{children}</authContext.Provider>;
}

AuthProvider.propTypes = {
	children: PropTypes.any
};

const useAuth = () => {
	return useContext(authContext);
};

export default useAuth;
