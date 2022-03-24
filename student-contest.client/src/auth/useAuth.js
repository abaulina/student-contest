import React, { createContext, useContext, useEffect, useState } from 'react';
import PropTypes from 'prop-types';
import {
	refreshToken as sendRefreshRequest,
	logout as sendLogoutRequest,
	login as sendLoginRequest
} from '../serverRequests';

const authContext = createContext();

function useProvideAuth() {
	const [isAuthenticated, setAuthenticated] = useState(
		accessToken ? true : false
	);
	const [accessToken, setAccessToken] = useState();

	useEffect(() => {
		refreshToken();
	});

	const login = async (loginCredentials) => {
		const accessToken = await sendLoginRequest(loginCredentials);
		setAuthenticated(true);
		setAccessToken(accessToken);
	};

	const logout = async () => {
		setAccessToken(null);
		setAuthenticated(false);
		await sendLogoutRequest();
	};

	const refreshToken = async () => {
		setTimeout(async () => {
			const accessToken = await sendRefreshRequest();
			setAccessToken(accessToken);
		}, 15 * 60000 - 1000);
	};

	return {
		isAuthenticated,
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
