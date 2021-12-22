import React, { createContext, useContext, useState, useEffect } from 'react';
import Cookies from 'js-cookie';
import PropTypes from 'prop-types';

const authContext = createContext();

function useProvideAuth() {
	const [isAuthenticated, setAuthenticated] = useState(
		Cookies.get('userEmail') ? true : false
	);

	const login = (email) => {
		setAuthenticated(true);
		Cookies.set('userEmail', email, { expires: 30 });
	};

	const logout = () => {
		setAuthenticated(false);
		Cookies.remove('userEmail');
	};

	useEffect(() => {
		const unsubscribe = () => {
			if (isAuthenticated) {
				setAuthenticated(true);
			} else {
				setAuthenticated(false);
			}
		};
		// Cleanup subscription on unmount
		return () => unsubscribe();
	}, []);

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
