import React, { useRef, useState } from 'react';
import { Link } from 'react-router-dom';
import toast from 'react-hot-toast';
import { useForm } from 'react-hook-form';
import hidePwdImg from '../../../assets/hide-password.svg';
import showPwdImg from '../../../assets/show-password.svg';
import './login.component.css';

export function Login() {
	const localStorage = window.localStorage;
	const [email, setEmail] = useState('');
	const [password, setPassword] = useState();
	const [isPasswordShown, setPasswordShown] = useState(false);
	const [isCapsWarningShown, setCapsWarningShown] = useState(false);
	const submitButton = useRef();
	const {
		register,
		handleSubmit,
		formState: { errors }
	} = useForm();

	const togglePassword = () => {
		setPasswordShown((isPasswordShown) => !isPasswordShown);
	};

	const capsLockWarning = (e) => {
		if (e.getModifierState('CapsLock') && !isCapsWarningShown) {
			toast.error('Caps lock is on', { icon: '❗' });
			setCapsWarningShown((isCapsWarningShown) => !isCapsWarningShown);
		}
	};

	const onPasswordInput = (e) => {
		setPassword(e.target.value);
	};

	const onEmailInput = (e) => {
		setEmail(e.target.value);
	};

	const onEnterPress = (e) => {
		if (e.charCode === 13) submitButton.current.click();
	};

	const doLogin = () => {
		setCapsWarningShown((isCapsWarningShown) => !isCapsWarningShown);
	};

	const validateEmail = (email) => {
		const users = JSON.parse(localStorage.getItem('Users')) || [];
		if (!users.some((user) => user.email === email)) return false;
		return true;
	};

	const validatePassword = () => {
		const users = JSON.parse(localStorage.getItem('Users')) || [];
		if (
			!users.some((user) => user.email === email && user.password === password)
		)
			return false;
		return true;
	};

	const KeepLoggedInCheckbox = () => (
		<div className='form-check mt-3'>
			<input
				className='form-check-input'
				type='checkbox'
				value=''
				id='flexCheckDefault'
			/>
			<label className='form-check-label' htmlFor='flexCheckDefault'>
				{' '}
				Log me out after
			</label>
		</div>
	);

	const SubmitButton = () => (
		<button
			ref={submitButton}
			type='submit'
			className='button default mt-4'
			onClick={handleSubmit(doLogin)}>
			Submit
		</button>
	);

	const ForgotPasswordLink = () => (
		<p className='forgot-password text-right'>
			<Link to=''>Forgot password?</Link>
		</p>
	);

	const NoAccountLink = () => (
		<p className='no-account text-right'>
			<Link to='/signup'>No account?</Link>
		</p>
	);

	return (
		<div className='auth-inner'>
			<h3>Welcome Back</h3>

			<div className='form-floating'>
				<input
					value={email}
					type='email'
					{...register('email', {
						validate: (value) =>
							validateEmail(value) === true ||
							'The email account that you tried to reach does not exist. Please try double-checking the email address'
					})}
					className={`form-control ${
						errors.email ? 'is-invalid' : ''
					} required`}
					id='floatingInput'
					placeholder='name@example.com'
					onKeyPress={onEnterPress}
					onChange={onEmailInput}
					autoFocus
				/>
				<label htmlFor='floatingInput'>Email address</label>
				<div className='invalid-feedback'>{errors.email?.message}</div>
			</div>

			<div className='form-floating d-flex log-in'>
				<input
					type={isPasswordShown ? 'text' : 'password'}
					className={`form-control ${errors.password ? 'is-invalid' : ''}`}
					{...register('password', {
						validate: (value) =>
							validatePassword(value) === true ||
							'The provided password is incorrect'
					})}
					id='floatingPassword'
					placeholder='Password'
					onKeyUp={capsLockWarning}
					onKeyPress={onEnterPress}
					onChange={onPasswordInput}
				/>
				<label htmlFor='floatingPassword'>Password</label>
				<img
					className='log-in'
					htmlFor='floatingPassword'
					alt='show or hide password'
					height='24'
					width='24'
					title={isPasswordShown ? 'Hide password' : 'Show password'}
					src={isPasswordShown ? hidePwdImg : showPwdImg}
					onClick={togglePassword}
				/>
				<div className='invalid-feedback'>{errors.password?.message}</div>
			</div>
			<KeepLoggedInCheckbox />
			<SubmitButton />
			<ForgotPasswordLink />
			<NoAccountLink />
		</div>
	);
}
