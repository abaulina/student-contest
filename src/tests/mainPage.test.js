import React from 'react';
import { unmountComponentAtNode } from 'react-dom';
import { render, act } from '@testing-library/react';
import Main from '../main/mainPage';

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

describe('Main when isAuthenticated', () => {
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
		act(() => {
			render(<Main />, container);
		});
	});

	it('redirects to user', () => {
		act(() => {
			render(<Main />, container);
		});

		expect(document.location.pathname).toMatch('/user');
	});
});

describe('Main when !isAuthenticated', () => {
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
		act(() => {
			render(<Main />, container);
		});
	});

	// it('displays image and login form', () => {
	// 	act(() => {
	// 		render(<Main />, container);
	// 	});

	// 	expect(document.location.pathname).toMatch('/user');
	// });
});
