import React from 'react';
import Cookies from 'js-cookie';
import { unmountComponentAtNode } from 'react-dom';
import { act } from 'react-dom/test-utils';
import { render, fireEvent } from '@testing-library/react';
import App from '../App';

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

it('renders without crashing', () => {
	render(<App />, container);
});

it('navigates home when you click the logo', () => {
	render(<App />, container);

	act(() => {
		fireEvent.click(document.getElementsByClassName('navbar-brand')[0]);
	});

	expect(document.location.pathname).toMatch('/');
});

it('navigates to login page on login navbar button click', () => {
	render(<App />, container);

	act(() => {
		fireEvent.click(document.querySelector('[data-testid=loginButton]'));
	});

	expect(document.location.pathname).toMatch('/login');
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

it('should remove cookie on LogOut button click', () => {
	const cookieRemoveSpy = jest
		.spyOn(Cookies, 'remove')
		.mockReturnValue('removed');
	const cookieGetSpy = jest
		.spyOn(Cookies, 'get')
		.mockReturnValue('ascd@list.ru');
	jest.spyOn(window.localStorage.__proto__, 'getItem').mockReturnValue({
		Users:
			'[{"email":"acsd@list.ru","password":"12345678","firstName":"Test","lastName":"User"}]'
	});
	render(<App />, container);

	act(() => {
		fireEvent.click(document.querySelector('[data-testid=logoutButton]'));
	});

	expect(cookieGetSpy).toHaveBeenCalled();
	expect(cookieRemoveSpy).toHaveBeenCalled();
	expect(cookieRemoveSpy.mock.results[0].value).toBe('removed');

	cookieGetSpy.mockRestore();
	cookieRemoveSpy.mockRestore();
});
