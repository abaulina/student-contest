import React from 'react';
import { render, screen } from '@testing-library/react';
import Main from '../main/mainPage';

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
		render(<Main />);
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
		render(<Main />);
	});

	it('displays image and login form', () => {
		render(<Main />);

		const image = screen.getByRole('img');
		const form = document.querySelector('[data-testid=loginForm]');
		expect(image).toBeInTheDocument();
		expect(form).toBeInTheDocument();
	});
});
