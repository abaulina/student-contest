import React from 'react';
import Cookies from 'js-cookie';
import { unmountComponentAtNode } from 'react-dom';
import { render, fireEvent, act } from '@testing-library/react';
import App from '../App';

describe('App and isAuthenticated', () => {
	let container = null;
	let cookieRemoveSpy = null;
	let cookieGetSpy = null;
	let localStorageGetSpy = null;

	beforeEach(() => {
		container = document.createElement('div');
		document.body.appendChild(container);
		cookieRemoveSpy = jest.spyOn(Cookies, 'remove').mockReturnValue('removed');
		cookieGetSpy = jest.spyOn(Cookies, 'get').mockReturnValue('acsd@list.ru');
		localStorageGetSpy = jest
			.spyOn(Storage.prototype, 'getItem')
			.mockReturnValue(
				'[{"email":"acsd@list.ru","password":"12345678","firstName":"Test","lastName":"User"}]'
			);
	});

	afterEach(() => {
		unmountComponentAtNode(container);
		container.remove();
		container = null;
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
		render(<App />, container);

		expect(document.location.pathname).toMatch('/');
	});

	it('navigates home when you click the logo', () => {
		act(() => {
			render(<App />, container);
			fireEvent.click(document.getElementsByClassName('navbar-brand')[0]);
		});

		expect(document.location.pathname).toMatch('/');
	});

	it('should remove cookie on LogOut button click', () => {
		act(() => {
			render(<App />, container);
			fireEvent.click(document.querySelector('[data-testid=logoutButton]'));
		});

		expect(cookieGetSpy).toHaveBeenCalled();
		expect(cookieRemoveSpy).toHaveBeenCalled();
		expect(cookieRemoveSpy.mock.results[0].value).toBe('removed');
	});

	// it('isAuthenticated should become false on LogOut button click', () => {
	// 	render(<App />, container);

	// 	act(() => {
	// 		fireEvent.click(document.querySelector('[data-testid=logoutButton]'));
	// 	});

	// 	expect(cookieGetSpy).toHaveBeenCalled();
	// 	expect(cookieRemoveSpy).toHaveBeenCalled();
	// 	expect(cookieRemoveSpy.mock.results[0].value).toBe('removed');
	// });

	it('LogOut button is visible when isAuthenticated', () => {
		let logOutButton = null;

		act(() => {
			render(<App />, container);
			logOutButton = document.querySelector('[data-testid=logoutButton]');
		});

		expect(logOutButton).toBeVisible();
	});

	it('LogIn button is invisible when isAuthenticated', () => {
		let loginButton = null;

		act(() => {
			render(<App />, container);
			loginButton = document.querySelector('[data-testid=loginButton]');
		});

		expect(loginButton).not.toBeVisible();
	});

	it('SignUp button is not rendered when isAuthenticated', () => {
		let signUpButton = null;

		act(() => {
			render(<App />, container);
			signUpButton = document.querySelector('[data-testid=signupButton]');
		});

		expect(signUpButton).toBeNull();
	});
});

describe('App and !isAuthenticated', () => {
	let container = null;

	beforeEach(() => {
		container = document.createElement('div');
		document.body.appendChild(container);
	});

	afterEach(() => {
		unmountComponentAtNode(container);
		container.remove();
		container = null;
	});

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
		render(<App />, container);

		expect(document.location.pathname).toMatch('/');
	});

	it('navigates home when you click the logo', () => {
		act(() => {
			render(<App />, container);
			fireEvent.click(document.getElementsByClassName('navbar-brand')[0]);
		});

		expect(document.location.pathname).toMatch('/');
	});

	it('redirects to login page on LogIn button click', () => {
		act(() => {
			render(<App />, container);
			fireEvent.click(document.querySelector('[data-testid=loginButton]'));
		});

		expect(document.location.pathname).toMatch('/login');
	});

	it('redirects to signup page on SignUp button click', () => {
		act(() => {
			render(<App />, container);
			fireEvent.click(document.querySelector('[data-testid=signupButton]'));
		});

		expect(document.location.pathname).toMatch('/signup');
	});

	it('SignUp button is visible when !isAuthenticated', () => {
		let signUpButton = null;

		act(() => {
			render(<App />, container);
			signUpButton = document.querySelector('[data-testid=signupButton]');
		});

		expect(signUpButton).toBeVisible();
	});

	it('LogIn button is visible when !isAuthenticated', () => {
		let loginButton = null;

		act(() => {
			render(<App />, container);
			loginButton = document.querySelector('[data-testid=loginButton]');
		});

		expect(loginButton).toBeVisible();
	});

	it('LogOut button is not rendered when !isAuthenticated', () => {
		let logOutButton = null;

		act(() => {
			render(<App />, container);
			logOutButton = document.querySelector('[data-testid=logoutButton]');
		});

		expect(logOutButton).toBeNull();
	});
});
