import React, { useEffect, useState } from 'react';
import PrivateRoute from './../privateRoute';
import { getUserInfo } from '../serverRequests';
import useAuth from '../auth/useAuth';
import LoadingSpinner from '../loading/loadingSpinner';
import './userAccount.css';

function UserAccount() {
	const [loggedInUser, setLoggedInUser] = useState([]);
	const [isLoading, setIsLoading] = useState(true);
	const auth = useAuth();

	useEffect(() => {
		const getLoggedInUserInfo = async () => {
			setIsLoading(true);
			const userInfo = await getUserInfo(auth.accessToken);
			setLoggedInUser(userInfo);
			setIsLoading(false);
		};

		getLoggedInUserInfo();
	}, []);

	return (
		<div className='d-flex user-account'>
			{isLoading ? (
				<LoadingSpinner />
			) : (
				<p className='not-found'>
					Nice to see you again, {loggedInUser.firstName}{' '}
					{loggedInUser.lastName}
				</p>
			)}
		</div>
	);
}

export function WrappedUserAccount() {
	return (
		<PrivateRoute>
			<UserAccount />
		</PrivateRoute>
	);
}

export default UserAccount;
