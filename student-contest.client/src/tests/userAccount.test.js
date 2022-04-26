import React from 'react';
import { render } from '@testing-library/react';
import UserAccount from '../user/userAccount';

jest.resetAllMocks();
jest.mock('../auth/useAuth', () => {
	const originalModule = jest.requireActual('../auth/useAuth');
	return {
		__esModule: true,
		...originalModule,
		default: () => ({
			isAuthenticated: true,
			accessToken: 'token',
			login: jest.fn,
			logout: jest.fn
		})
	};
});

it('renders without crashing', () => {
	render(<UserAccount />);
});

it('shows loading spinner while fetching', () => {
	render(<UserAccount />);

	expect(document.querySelector('.loading-spinner')).not.toBeNull();
});
