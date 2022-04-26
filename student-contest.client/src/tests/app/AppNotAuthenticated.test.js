import React from 'react';
import { render, fireEvent, screen } from '@testing-library/react';
import App from '../../App';

jest.mock('../../auth/useAuth', () => {
	const originalModule = jest.requireActual('../../auth/useAuth');
	return {
		__esModule: true,
		...originalModule,
		default: () => ({
			isAuthenticated: false,
			login: jest.fn,
			logout: jest.fn
		})
	};
});

describe('App when !isAuthenticated', () => {
	it('renders without crashing', () => {
		render(<App />);
	});

	it('SignUp button is visible when !isAuthenticated', () => {
		render(<App />);

		expect(screen.getByText(/sign up/i)).toBeVisible();
	});

	it('LogIn button is visible when !isAuthenticated', () => {
		render(<App />);

		expect(screen.getByText(/log in/i)).toBeVisible();
	});

	it('LogOut button is not rendered when !isAuthenticated', () => {
		render(<App />);

		expect(screen.queryByText(/log out/i)).toBeNull();
	});

	it('redirects to signup page on no account click', async () => {
		render(<App />);
		fireEvent.click(screen.getByText(/no account/i));

		expect(
			await screen.findByPlaceholderText(/last name/i)
		).toBeInTheDocument();
	});

	it('redirects to login page on LogIn button click', () => {
		render(<App />);
		fireEvent.click(screen.getByText(/log in/i));

		expect(
			document.querySelector('[data-testid=loginForm]')
		).toBeInTheDocument();
	});

	it('redirects to signup page on SignUp button click', async () => {
		render(<App />);
		fireEvent.click(screen.getByText(/sign up/i));

		expect(
			await screen.findByPlaceholderText(/last name/i)
		).toBeInTheDocument();
	});
});
