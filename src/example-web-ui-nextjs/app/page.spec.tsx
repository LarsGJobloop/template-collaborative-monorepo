import { it } from 'vitest';
import { render } from '@testing-library/react';
import Home from './page';

it('renders without crashing', () => {
  render(<Home />);
});
