import React from 'react';
import { render, screen } from '@testing-library/react';
import App from '../../App';

jest.resetAllMocks();
jest.mock('../../auth/useAuth', () => {
	const originalModule = jest.requireActual('../../auth/useAuth');
	return {
		__esModule: true,
		...originalModule,
		default: () => ({
			accessToken: 'token',
			isAuthenticated: true,
			login: jest.fn,
			logout: jest.fn
		})
	};
});

describe('App when isAuthenticated', () => {
	it('renders without crashing', () => {
		render(<App />);
	});

	it('LogOut button is visible when isAuthenticated', () => {
		render(<App />);

		expect(screen.getByText(/log out/i)).toBeVisible();
	});

	it('LogIn button is invisible when isAuthenticated', () => {
		render(<App />);

		expect(document.getElementsByClassName('btn invisible')).not.toBeNull();
	});

	it('SignUp button is not rendered when isAuthenticated', () => {
		render(<App />);

		expect(screen.queryByText(/sign up/i)).toBeNull();
	});
});
