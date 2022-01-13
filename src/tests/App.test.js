import React from 'react';
import Cookies from 'js-cookie';
import '@testing-library/jest-dom';
import { render, fireEvent, screen } from '@testing-library/react';
import App from '../App';

describe('App when isAuthenticated', () => {
	let cookieRemoveSpy = null;
	let cookieGetSpy = null;
	let localStorageGetSpy = null;

	beforeEach(() => {
		cookieRemoveSpy = jest.spyOn(Cookies, 'remove').mockReturnValue('removed');
		cookieGetSpy = jest
			.spyOn(Cookies, 'get')
			.mockReturnValue('test@example.com');
		localStorageGetSpy = jest
			.spyOn(Storage.prototype, 'getItem')
			.mockReturnValue(
				'[{"email":"test@example.com","password":"12345678","firstName":"Test","lastName":"User"}]'
			);
	});

	afterEach(() => {
		cookieGetSpy.mockRestore();
		cookieRemoveSpy.mockRestore();
		localStorageGetSpy.mockRestore();
	});

	jest.mock('../auth/useAuth', () => {
		const originalModule = jest.requireActual('../auth/useAuth');
		return {
			__esModule: true,
			...originalModule,
			default: () => ({
				isAuthenticated: true,
				login: jest.fn,
				logout: jest.fn
			})
		};
	});

	it('renders without crashing', () => {
		render(<App />);
	});

	it('should remove cookie on LogOut button click', () => {
		render(<App />);
		fireEvent.click(screen.getByText(/log out/i));

		expect(cookieGetSpy).toHaveBeenCalled();
		expect(cookieRemoveSpy).toHaveBeenCalled();
		expect(cookieRemoveSpy.mock.results[0].value).toBe('removed');
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

describe('App when !isAuthenticated', () => {
	jest.mock('../auth/useAuth', () => {
		const originalModule = jest.requireActual('../auth/useAuth');
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
});
